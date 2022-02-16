using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhantomKit.Helpers;

public static class DataManipulation
{
    /// <summary>
    ///     Return settings that try to make the file as compatible as possible for human editors (apart from the encrypted
    ///     fields)
    /// </summary>
    /// <returns></returns>
    public static JsonSerializerOptions NewSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = Constants.PrettyPrintJson,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            IncludeFields = false /* default */,
            IgnoreReadOnlyFields = false /* default */,
            IgnoreReadOnlyProperties = false /* default */,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        };
    }

    public static IEnumerable<byte> SerializeObject(object value)
    {
        return JsonSerializer.Serialize(
                value: value,
                options: NewSerializerOptions())
            .ToCharArray()
            .Select(selector: c => (byte) c);
    }

    /// <summary>
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Deserialize<T>(in IEnumerable<byte> data)
    {
        return JsonSerializer.Deserialize<T>(
            json: new string(value: data.ToArray().Select(selector: b => (char) b).ToArray()),
            options: NewSerializerOptions())!;
    }

    /// <summary>
    /// </summary>
    /// <param name="data"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object DeserializeToObject(in IEnumerable<byte> data, Type type)
    {
        return JsonSerializer.Deserialize(
            json: new string(value: data.ToArray().Select(selector: b => (char) b).ToArray()),
            options: NewSerializerOptions(),
            returnType: type)!;
    }


    public static IEnumerable<byte> GenerateSalt()
    {
        Debug.Assert(condition: Constants.PasswordSaltSize >= 8);
        var passwordSalt = new byte[Constants.PasswordSaltSize];
        var rngCsp = new Random();
        rngCsp.NextBytes(buffer: passwordSalt);
        return passwordSalt;
    }

    public static uint ComputeHashedCrc32(int dataLength, IEnumerable<byte> unencryptedObjectData)
    {
        var unencryptedObjectDataArray = unencryptedObjectData
            .ToArray();

        var keyNameBytes = BitConverter
            .GetBytes(value: dataLength)
            .Concat(second: unencryptedObjectDataArray)
            .ToArray();
        return Crc32.ComputeChecksum(bytes: keyNameBytes);
    }

    public static bool VerifyHashedCrc32(uint expectedCrc32, int dataLength,
        IEnumerable<byte> unencryptedObjectData, out uint calculatedCrc32)
    {
        calculatedCrc32 = ComputeHashedCrc32(dataLength: dataLength,
            unencryptedObjectData: unencryptedObjectData);
        return expectedCrc32 == calculatedCrc32;
    }

    public static Aes GetAes(IEnumerable<byte>? iv = null)
    {
        var aes = Aes.Create();
        aes.KeySize = Constants.AesKeySize;
        aes.Mode = Constants.AesCipherMode;
        aes.Padding = Constants.AesPaddingMode;
        if (iv is not null)
            aes.IV = iv.ToArray();
        else
            aes.GenerateIV();
        return aes;
    }

    public static Aes SetAesKey(Aes aes, SecureString password, IEnumerable<byte> passwordSalt)
    {
        var saltArray = passwordSalt.ToArray();
        Debug.Assert(condition: saltArray.Length >= 8);

        var stringPointer = Marshal.SecureStringToBSTR(s: password);
        var insecurePasswordString = Marshal.PtrToStringBSTR(ptr: stringPointer);
        Marshal.ZeroFreeBSTR(s: stringPointer);

        var pbkdf2 = new Rfc2898DeriveBytes(
            password: insecurePasswordString,
            salt: saltArray,
            iterations: Constants.Pbkdf2Iterations,
            hashAlgorithm: HashAlgorithmName.SHA256);

        aes.Key = pbkdf2.GetBytes(cb: aes.KeySize / 8);
        return aes;
    }

    public static IEnumerable<byte> Encrypt(Aes aes, SecureString password, IEnumerable<byte> passwordSalt,
        IEnumerable<byte> input)
    {
        return Transform(input: input,
            cryptoTrans: SetAesKey(
                aes: aes,
                password: password,
                passwordSalt: passwordSalt).CreateEncryptor());
    }

    public static IEnumerable<byte> Decrypt(Aes aes, SecureString password, IEnumerable<byte> passwordSalt,
        IEnumerable<byte> input)
    {
        return Transform(input: input,
            cryptoTrans: SetAesKey(
                aes: aes,
                password: password,
                passwordSalt: passwordSalt).CreateDecryptor());
    }

    private static IEnumerable<byte> Transform(IEnumerable<byte> input, ICryptoTransform cryptoTrans)
    {
        var inputArray = input.ToArray();
        if (cryptoTrans.CanTransformMultipleBlocks)
            return cryptoTrans.TransformFinalBlock(inputBuffer: inputArray,
                inputOffset: 0,
                inputCount: inputArray.Length);

        using var ms = new MemoryStream();
        using var cs = new CryptoStream(
            stream: ms,
            transform: cryptoTrans,
            mode: CryptoStreamMode.Write);
        cs.Write(buffer: inputArray,
            offset: 0,
            count: inputArray.Length);
        cs.FlushFinalBlock();

        return ms.ToArray();
    }


    public static string Decompress(string input)
    {
        var compressed = Convert.FromBase64String(s: input);
        return new string(value: Decompress(input: compressed).Select(selector: b => (char) b).ToArray());
    }

    public static string Compress(string input, CompressionLevel compressionLevel)
    {
        var encoded = input.Select(selector: c => (byte) c).ToArray();
        var compressed = Compress(input: encoded,
            compressionLevel: compressionLevel);
        return Convert.ToBase64String(inArray: compressed);
    }

    public static byte[] Decompress(byte[] input)
    {
        using var source = new MemoryStream(buffer: input);
        var lengthBytes = new byte[4];
        source.Read(buffer: lengthBytes,
            offset: 0,
            count: 4);

        var length = BitConverter.ToInt32(value: lengthBytes,
            startIndex: 0);
        using var decompressionStream = new GZipStream(stream: source,
            mode: CompressionMode.Decompress);
        var result = new byte[length];
        decompressionStream.Read(buffer: result,
            offset: 0,
            count: length);
        return result;
    }

    public static byte[] Compress(byte[] input, CompressionLevel compressionLevel)
    {
        using var result = new MemoryStream();
        var lengthBytes = BitConverter.GetBytes(value: input.Length);
        result.Write(buffer: lengthBytes,
            offset: 0,
            count: 4);

        using (var compressionStream = new GZipStream(stream: result,
                   compressionLevel: CompressionLevel.SmallestSize))
        {
            compressionStream.Write(buffer: input,
                offset: 0,
                count: input.Length);
            compressionStream.Flush();
        }

        return result.ToArray();
    }
}