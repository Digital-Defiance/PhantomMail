using System.Text.Json.Serialization;
using PhantomKit.Exceptions;
using PhantomKit.Helpers;

namespace PhantomKit.Models;

/// <summary>
///     Contains encrypted data for any serializable object type
/// </summary>
[Serializable]
public record EncryptableObjectSetting
{
    public EncryptableObjectSetting()
    {
        this.Crc32 = 0;
        this.Value = string.Empty;
        this.ValueTypeBase64 = string.Empty;
    }

    /// <summary>
    /// </summary>
    /// <param name="crc32"></param>
    /// <param name="dataLength"></param>
    /// <param name="valueBytes"></param>
    /// <param name="valueType"></param>
    /// <exception cref="InvalidOperationException"></exception>
    protected EncryptableObjectSetting(uint crc32, int dataLength, byte[] valueBytes,
        Type valueType)
    {
        this.Crc32 = crc32;
        this.DataLength = dataLength;
        this.Value = Convert.ToBase64String(
            inArray: DataManipulation.Compress(
                input: valueBytes,
                compressionLevel: Constants.DataCompressionLevel));
        this.ValueTypeBase64 = Convert.ToBase64String(
            inArray: DataManipulation.Compress(
                input: Utilities.TypeNameToString(
                    type: valueType).Select(selector: c => (byte) c).ToArray(),
                compressionLevel: Constants.TypeCompressionLevel));
    }

    /// <summary>
    /// </summary>
    /// <param name="crc32"></param>
    /// <param name="dataLength"></param>
    /// <param name="base64Value"></param>
    /// <param name="base64ValueType"></param>
    protected EncryptableObjectSetting(uint crc32, int dataLength,
        string base64Value,
        string base64ValueType)
    {
        this.Crc32 = crc32;
        this.DataLength = dataLength;
        this.Value = base64Value;
        this.ValueTypeBase64 = base64ValueType;
    }

    [JsonIgnore] public virtual bool Encrypted => false;

    /// <summary>
    ///     crc32 of the unencrypted data
    /// </summary>
    [JsonInclude]
    [JsonPropertyName(name: "c")]
    public uint Crc32 { get; private set; }

    /// <summary>
    ///     base64 encoded data
    /// </summary>
    [JsonInclude]
    [JsonPropertyName(name: "v")]
    public string Value { get; private set; }

    /// <summary>
    ///     UnencryptedDataLength of unencrypted data
    /// </summary>
    [JsonInclude]
    [JsonPropertyName(name: "l")]
    public int DataLength { get; private set; }

    /// <summary>
    ///     Base64 encoded string
    /// </summary>
    [JsonInclude]
    [JsonPropertyName(name: "t")]
    public string ValueTypeBase64 { get; private set; }

    [JsonIgnore]
    public IEnumerable<byte> ValueBytes => DataManipulation.Decompress(input: Convert.FromBase64String(
        s: this.Value));

    [JsonIgnore]
    public Type ValueType => Type.GetType(
        typeName: new string(value: DataManipulation.Decompress(
            input: Convert.FromBase64String(
                s: this.ValueTypeBase64)).Select(selector: b => (char) b).ToArray()),
        throwOnError: true,
        ignoreCase: false)!;

    [JsonIgnore]
    public bool ValidCrc32 => DataManipulation.VerifyHashedCrc32(
        expectedCrc32: this.Crc32,
        dataLength: this.DataLength,
        unencryptedObjectData: this.ValueBytes,
        calculatedCrc32: out var _);

    public T Object<T>()
    {
        if (this.Encrypted)
            throw new NotSupportedException();
        if (!DataManipulation.VerifyHashedCrc32(
                expectedCrc32: this.Crc32,
                dataLength: this.DataLength,
                unencryptedObjectData: this.ValueBytes,
                calculatedCrc32: out var calculatedCrc32))
            throw new InvalidSettingsException(
                paramName: nameof(this.Crc32),
                paramValue: $"crc32 mismatch: {this.Crc32} != {calculatedCrc32}");
        return DataManipulation.Deserialize<T>(
            data: this.ValueBytes);
    }

    public object Object()
    {
        if (this.Encrypted)
            throw new NotSupportedException();
        if (!DataManipulation.VerifyHashedCrc32(
                expectedCrc32: this.Crc32,
                dataLength: this.DataLength,
                unencryptedObjectData: this.ValueBytes,
                calculatedCrc32: out var calculatedCrc32))
            throw new InvalidSettingsException(
                paramName: nameof(this.Crc32),
                paramValue: $"crc32 mismatch: {this.Crc32} != {calculatedCrc32}");
        return DataManipulation.DeserializeToObject(
            data: this.ValueBytes,
            type: this.ValueType);
    }

    /// <summary>
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static EncryptableObjectSetting Create<T>(T value)
    {
        return CreateFromObject(
            value: value!,
            valueType: typeof(T));
    }

    /// <summary>
    ///     Internal passthrough bypasses deserialization/re-serialization of object
    /// </summary>
    /// <param name="unencryptedObjectData"></param>
    /// <param name="originalLength"></param>
    /// <param name="valueType"></param>
    /// <returns></returns>
    public static EncryptableObjectSetting CreateFromBytes(
        IEnumerable<byte> unencryptedObjectData,
        int originalLength,
        Type valueType)
    {
        var unencryptedBytes = unencryptedObjectData.ToArray();

        return new EncryptableObjectSetting(
            crc32: DataManipulation.ComputeHashedCrc32(
                dataLength: originalLength,
                unencryptedObjectData: unencryptedBytes),
            dataLength: originalLength,
            valueBytes: unencryptedBytes,
            valueType: valueType);
    }

    /// <summary>
    ///     CreateFromBytes an encrypted setting (encrypts during creation)
    /// </summary>
    /// <param name="value"></param>
    /// <param name="valueType"></param>
    /// <returns></returns>
    public static EncryptableObjectSetting CreateFromObject(
        object value,
        Type valueType)
    {
        // secureValue's type must be valueType
        if (valueType != value.GetType())
            throw new InvalidSettingsException(paramName: nameof(ValueType),
                paramValue:
                $"ValueTypeBase64 mismatch, expected: {valueType.AssemblyQualifiedName}, received: {value.GetType().AssemblyQualifiedName}");

        var unencryptedBytes = DataManipulation.SerializeObject(value: value).ToArray();
        return CreateFromBytes(
            unencryptedObjectData: unencryptedBytes,
            valueType: valueType,
            originalLength: unencryptedBytes.Length);
    } // end of CreateFromBytes
} // end of class EncryptedObjectSetting