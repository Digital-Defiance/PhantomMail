using System.Security;
using System.Text.Json.Serialization;
using PhantomKit.Exceptions;
using PhantomKit.Helpers;

namespace PhantomKit.Models;

/// <summary>
///     Contains encrypted data for any serializable object type
/// </summary>
[Serializable]
public record EncryptedObjectSetting : EncryptableObjectSetting
{
    public EncryptedObjectSetting()
    {
        this.IvBase64 = string.Empty;
        this.PasswordSaltBase64 = string.Empty;
    }

    /// <summary>
    /// </summary>
    /// <param name="crc32"></param>
    /// <param name="unencryptedDataLength"></param>
    /// <param name="iv"></param>
    /// <param name="salt"></param>
    /// <param name="encryptedValue"></param>
    /// <param name="valueType"></param>
    /// <exception cref="InvalidOperationException"></exception>
    protected EncryptedObjectSetting(uint crc32, int unencryptedDataLength, byte[] iv,
        byte[] salt, byte[] encryptedValue,
        Type valueType) : base(crc32: crc32,
        dataLength: unencryptedDataLength,
        valueBytes: encryptedValue,
        valueType: valueType)
    {
        this.IvBase64 = Convert.ToBase64String(inArray: iv);
        this.PasswordSaltBase64 = Convert.ToBase64String(inArray: salt);
    }

    /// <summary>
    /// </summary>
    /// <param name="crc32"></param>
    /// <param name="unencryptedDataLength"></param>
    /// <param name="base64IvBase64"></param>
    /// <param name="base64PasswordSaltBase64"></param>
    /// ">
    /// <param name="base64EncryptedValue"></param>
    /// <param name="base64ValueType"></param>
    protected EncryptedObjectSetting(uint crc32, int unencryptedDataLength,
        string base64IvBase64, string base64PasswordSaltBase64,
        string base64EncryptedValue,
        string base64ValueType) : base(
        crc32: crc32,
        dataLength: unencryptedDataLength,
        base64Value: base64EncryptedValue,
        base64ValueType: base64ValueType)
    {
        this.IvBase64 = base64IvBase64;
        this.PasswordSaltBase64 = base64PasswordSaltBase64;
    }

    [JsonIgnore] public EncryptableObjectSetting AsEncryptableObjectSetting => this;

    [JsonIgnore] public override bool Encrypted => this.IvBase64.Length > 0 && this.PasswordSaltBase64.Length > 0;

    /// <summary>
    ///     base64 encoded iv
    /// </summary>
    [JsonInclude]
    [JsonPropertyName(name: "i")]
    public string IvBase64 { get; private set; }

    /// <summary>
    ///     base64 encoded salt for the password
    /// </summary>
    [JsonInclude]
    [JsonPropertyName(name: "s")]
    public string PasswordSaltBase64 { get; private set; }

    [JsonIgnore] public IEnumerable<byte> Iv => Convert.FromBase64String(s: this.IvBase64);

    [JsonIgnore] public IEnumerable<byte> PasswordSalt => Convert.FromBase64String(s: this.PasswordSaltBase64);

    /// <summary>
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Decrypt<T>(SecureString key)
    {
        return DataManipulation.Deserialize<T>(
            data: DecryptAsBytes(
                key: key,
                crc32: this.Crc32,
                dataLength: this.DataLength,
                iv: this.Iv,
                passwordSalt: this.PasswordSalt,
                encryptedBytes: this.ValueBytes));
    }

    private new T Object<T>()
    {
        throw new NotSupportedException();
    }

    private new object Object()
    {
        throw new NotSupportedException();
    }


    public object Decrypt(SecureString key)
    {
        return DataManipulation.DeserializeToObject(
            data: DecryptAsBytes(
                key: key,
                crc32: this.Crc32,
                dataLength: this.DataLength,
                iv: this.Iv,
                passwordSalt: this.PasswordSalt,
                encryptedBytes: this.ValueBytes),
            type: this.ValueType);
    }

    /// <summary>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="crc32"></param>
    /// <param name="dataLength"></param>
    /// <param name="iv"></param>
    /// <param name="passwordSalt"></param>
    /// <param name="encryptedBytes"></param>
    /// <returns></returns>
    /// <exception cref="EncryptedSettingException"></exception>
    public static IEnumerable<byte> DecryptAsBytes(SecureString key, uint crc32, int dataLength, IEnumerable<byte> iv,
        IEnumerable<byte> passwordSalt, IEnumerable<byte> encryptedBytes)
    {
        var aes = DataManipulation.GetAes(iv: iv);
        var decryptedBytes = DataManipulation.Decrypt(
            aes: aes,
            password: key,
            passwordSalt: passwordSalt,
            input: encryptedBytes).ToArray();

        if (!DataManipulation.VerifyHashedCrc32(
                expectedCrc32: crc32,
                dataLength: dataLength,
                unencryptedObjectData: decryptedBytes,
                calculatedCrc32: out var calculatedCrc32))
            throw new EncryptedSettingException(paramName: nameof(crc32),
                paramValue: $"Decryption failed, crc32 check failed {crc32} != {calculatedCrc32}");

        return decryptedBytes;
    }

    /// <summary>
    /// </summary>
    /// <param name="vaultKey"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static EncryptedObjectSetting Create<T>(in SecureString vaultKey, T value)
    {
        return CreateFromObject(
            vaultKey: vaultKey,
            value: value!,
            valueType: typeof(T));
    }

    /// <summary>
    ///     Internal passthrough bypasses deserialization/re-serialization of object
    /// </summary>
    /// <param name="vaultKey"></param>
    /// <param name="unencryptedObjectData"></param>
    /// <param name="originalLength"></param>
    /// <param name="valueType"></param>
    /// <returns></returns>
    public static EncryptedObjectSetting CreateFromBytes(in SecureString vaultKey,
        IEnumerable<byte> unencryptedObjectData,
        int originalLength,
        Type valueType)
    {
        var passwordSaltBytes = DataManipulation.GenerateSalt().ToArray();
        var unencryptedBytes = unencryptedObjectData.ToArray();
        var aes = DataManipulation.GetAes();
        var encryptedBytes = DataManipulation.Encrypt(
            aes: aes,
            password: vaultKey,
            passwordSalt: passwordSaltBytes,
            input: unencryptedBytes).ToArray();

        return new EncryptedObjectSetting(
            crc32: DataManipulation.ComputeHashedCrc32(
                dataLength: originalLength,
                unencryptedObjectData: unencryptedBytes),
            unencryptedDataLength: originalLength,
            iv: aes.IV,
            salt: passwordSaltBytes,
            encryptedValue: encryptedBytes,
            valueType: valueType);
    }

    /// <summary>
    ///     CreateFromBytes an encrypted setting (encrypts during creation)
    /// </summary>
    /// <param name="vaultKey">Vault key is not kept</param>
    /// <param name="value"></param>
    /// <param name="valueType"></param>
    /// <returns></returns>
    public static EncryptedObjectSetting CreateFromObject(in SecureString vaultKey,
        object value,
        Type valueType)
    {
        // secureValue's type must be valueType
        if (valueType != value.GetType())
            throw new EncryptedSettingException(paramName: nameof(ValueType),
                paramValue:
                $"ValueTypeBase64 mismatch, expected: {valueType.AssemblyQualifiedName}, received: {value.GetType().AssemblyQualifiedName}");

        var unencryptedBytes = DataManipulation.SerializeObject(value: value).ToArray();
        return CreateFromBytes(
            vaultKey: vaultKey,
            unencryptedObjectData: unencryptedBytes,
            valueType: valueType,
            originalLength: unencryptedBytes.Length);
    } // end of CreateFromBytes
} // end of class EncryptedObjectSetting