using System.Runtime.InteropServices;
using System.Security;
using PhantomKit.Exceptions;
using PhantomKit.Helpers;

namespace PhantomKit.Models.Settings;

/// <summary>
///     An encrypted string which restores to a SecureString.
/// </summary>
[Serializable]
public sealed record EncryptedSecureStringSetting : EncryptedObjectSetting
{
    public EncryptedSecureStringSetting()
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="crc32"></param>
    /// <param name="unencryptedDataLength"></param>
    /// <param name="iv"></param>
    /// <param name="salt"></param>
    /// <param name="encryptedValue"></param>
    private EncryptedSecureStringSetting(uint crc32, int unencryptedDataLength, byte[] iv, byte[] salt,
        byte[] encryptedValue) :
        base(
            crc32: crc32,
            unencryptedDataLength: unencryptedDataLength,
            iv: iv,
            salt: salt,
            encryptedValue: encryptedValue,
            valueType: typeof(SecureString))
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="keyName"></param>
    /// <param name="vaultKey"></param>
    /// <param name="secureValue"></param>
    /// <returns></returns>
    public static EncryptedSecureStringSetting CreateFromSecureString(string keyName, in SecureString vaultKey,
        SecureString secureValue)
    {
        var passwordSalt = DataManipulation.GenerateSalt().ToArray();
        var stringPointer = Marshal.SecureStringToBSTR(s: secureValue);
        var secureValueString = Marshal.PtrToStringBSTR(ptr: stringPointer);
        Marshal.ZeroFreeBSTR(s: stringPointer);

        var unencryptedBytes = secureValueString.ToCharArray().Select(selector: c => (byte) c).ToArray();
        var length = unencryptedBytes.Length;
        var crc32 = DataManipulation.ComputeHashedCrc32(
            dataLength: length,
            unencryptedObjectData: unencryptedBytes);
        // write the unencrypted string to the encrypted object stream through the transformation crypto stream

        var aes = DataManipulation.GetAes();
        var encryptedBytes = DataManipulation.Encrypt(
            aes: aes,
            password: vaultKey,
            passwordSalt: passwordSalt,
            input: unencryptedBytes).ToArray();

        return new EncryptedSecureStringSetting(
            crc32: crc32,
            unencryptedDataLength: length,
            iv: aes.IV,
            salt: passwordSalt,
            encryptedValue: encryptedBytes);
    } // end of CreateFromBytes

    /// <summary>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="EncryptedSettingException"></exception>
    public new SecureString Decrypt(SecureString key)
    {
        var passwordSalt = this.PasswordSalt.ToArray();
        var encryptedBytes = this.ValueBytes.ToArray();

        var aes = DataManipulation.GetAes(iv: this.Iv);
        var decryptedBytes = DataManipulation.Decrypt(
            aes: aes,
            password: key,
            passwordSalt: passwordSalt,
            input: encryptedBytes).ToArray();

        if (!DataManipulation.VerifyHashedCrc32(
                expectedCrc32: this.Crc32,
                dataLength: this.DataLength,
                unencryptedObjectData: decryptedBytes,
                calculatedCrc32: out var calculatedCrc32))
            throw new EncryptedSettingException(paramName: nameof(this.Crc32),
                paramValue: $"Decryption failed, crc32 check failed {this.Crc32} != {calculatedCrc32}");

        var secureString = new SecureString();
        foreach (var c in decryptedBytes)
            secureString.AppendChar(c: (char) c);
        secureString.MakeReadOnly();
        return secureString;
    } // end of Decrypt
} // end of class EncryptedStringSetting