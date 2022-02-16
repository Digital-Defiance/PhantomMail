using System;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.Json;
using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhantomKit.Helpers;
using PhantomKit.Models;
using PhantomKit.Models.Settings;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace PhantomKit.Test;

[TestClass]
public class UtilitiesTests
{
    private static SecureString NewVaultKey(out string plainPassword)
    {
        var secureString = new SecureString();
        plainPassword = new Faker().Random.AlphaNumeric(length: 16)!;
        foreach (var c in plainPassword) secureString.AppendChar(c: c);
        secureString.MakeReadOnly();
        return secureString;
    }

    [TestMethod]
    public void TestHashedCrc32()
    {
        var expectedLength = new Faker().Random.Number(min: 1,
            max: 1024);
        var randomData = new Faker().Random.Bytes(count: expectedLength);

        var crc32 = DataManipulation.ComputeHashedCrc32(
            dataLength: expectedLength,
            unencryptedObjectData: randomData);
        Assert.IsTrue(condition: DataManipulation.VerifyHashedCrc32(
            expectedCrc32: crc32,
            dataLength: expectedLength,
            unencryptedObjectData: randomData,
            calculatedCrc32: out var calculatedCrc32Correct));

        // change a random byte
        var randomByte = new Faker().Random.Number(min: 0,
            max: 255);
        var changeIndex = new Faker().Random.Number(min: 0,
            max: randomData.Length - 1);
        var savedByte = randomData[changeIndex];
        randomData[changeIndex] = (byte) randomByte;

        // ensure the new crc32 is not the same
        var newCrc32 = DataManipulation.ComputeHashedCrc32(
            dataLength: expectedLength,
            unencryptedObjectData: randomData);
        Assert.AreNotEqual(
            notExpected: newCrc32,
            actual: crc32);
        // ensure the hash is not valid on the changed data
        Assert.IsFalse(condition: DataManipulation.VerifyHashedCrc32(
            expectedCrc32: crc32,
            dataLength: expectedLength,
            unencryptedObjectData: randomData,
            calculatedCrc32: out var calculatedCrc32Incorrect));
        Assert.AreEqual(
            expected: newCrc32,
            actual: calculatedCrc32Incorrect);

        // change the byte back and re-verify
        randomData[changeIndex] = savedByte;
        Assert.IsTrue(condition: DataManipulation.VerifyHashedCrc32(
            expectedCrc32: crc32,
            dataLength: expectedLength,
            unencryptedObjectData: randomData,
            calculatedCrc32: out var calculatedCrc32CorrectAgain));
        Assert.AreEqual(
            expected: crc32,
            actual: calculatedCrc32CorrectAgain);
    }

    [TestMethod]
    public void TestEncryptDecrypt()
    {
        var passwordSalt = DataManipulation.GenerateSalt().ToArray();
        using var secureString = NewVaultKey(plainPassword: out var plainPassword);
        var expectedLength = new Random().Next(minValue: 1,
            maxValue: 1024);
        var randomBytes = new Faker().Random.Bytes(count: expectedLength);
        var aes = DataManipulation.GetAes();
        var encryptedBytes = DataManipulation.Encrypt(
            aes: aes,
            password: secureString,
            passwordSalt: passwordSalt,
            input: randomBytes);

        var aes2 = DataManipulation.GetAes(iv: aes.IV /* pass the IV to the second AES */);
        var decryptedBytes = DataManipulation.Decrypt(
            aes: aes2,
            password: secureString,
            passwordSalt: passwordSalt,
            input: encryptedBytes);
        Assert.IsTrue(condition: randomBytes.SequenceEqual(second: decryptedBytes));
    }

    [TestMethod]
    public void TestCompressDecompressString()
    {
        var randomCompressionLevel = new Faker().PickRandom<CompressionLevel>();
        var plainText = new Faker().Random.AlphaNumeric(length: 16)!;
        // ReSharper disable once SuggestVarOrType_BuiltInTypes
        string compressedText = DataManipulation.Compress(
            input: plainText,
            compressionLevel: randomCompressionLevel);
        // ReSharper disable once SuggestVarOrType_BuiltInTypes
        string decompressedText = DataManipulation.Decompress(
            input: compressedText);
        Assert.AreNotEqual(notExpected: compressedText,
            actual: decompressedText);
        Assert.AreEqual(
            expected: plainText,
            actual: decompressedText);
    }

    [TestMethod]
    public void TestCompressDecompressBytes()
    {
        var randomCompressionLevel = new Faker().PickRandom<CompressionLevel>();
        var plainText = new Faker().Random.AlphaNumeric(length: 16)!;
        // ReSharper disable once SuggestVarOrType_BuiltInTypes
        byte[] compressedBytes = DataManipulation.Compress(
            input: plainText.ToCharArray().Select(selector: c => (byte) c).ToArray(),
            compressionLevel: randomCompressionLevel);
        // ReSharper disable once SuggestVarOrType_BuiltInTypes
        byte[] decompressedBytes = DataManipulation.Decompress(
            input: compressedBytes);
        Assert.AreNotEqual(notExpected: compressedBytes,
            actual: decompressedBytes);
        Assert.AreEqual(
            expected: plainText,
            actual: new string(value: decompressedBytes.ToArray().Select(selector: b => (char) b).ToArray()));
    }

    [TestMethod]
    public void TestEncryptedObject()
    {
        var expectedObjectName = new Faker().Lorem.Word();
        var expectedLength = new Faker().Random.Number(min: 1,
            max: 1024);
        var expectedValueA = new string(value: new Faker().Random.Bytes(count: expectedLength)!
            .Select(selector: b => (char) b).ToArray());
        var expectedValueB = new Faker().Random.Int(
            min: int.MinValue,
            max: int.MaxValue);
        var expectedObject = new TestObject
        {
            ValueA = expectedValueA,
            ValueB = expectedValueB,
        };
        using var vaultKey = NewVaultKey(plainPassword: out var _);

        var expectedObjectBytes = DataManipulation.SerializeObject(value: expectedObject).ToArray();
        var expectedCrc32 = DataManipulation.ComputeHashedCrc32(
            dataLength: expectedObjectBytes.Length,
            unencryptedObjectData: expectedObjectBytes);

        var encryptedObject = EncryptedObjectSetting.CreateFromObject(
            vaultKey: vaultKey,
            value: expectedObject,
            valueType: typeof(TestObject));

        Assert.AreEqual(
            expected: expectedCrc32,
            actual: encryptedObject.Crc32);

        // decrypting verifies the crc of the decrypted data
        var decryptedObject = encryptedObject.Decrypt<TestObject>(key: vaultKey);
        // ValueType verifies the file type is valid
        Assert.AreEqual(
            expected: expectedObject.GetType(),
            actual: encryptedObject.ValueType);
        Assert.AreEqual(
            expected: expectedValueA,
            actual: decryptedObject.ValueA);
        Assert.AreEqual(
            expected: expectedValueB,
            actual: decryptedObject.ValueB);

        // ensure data survives serialization and deserialization

        var json = JsonSerializer.Serialize(
            value: encryptedObject,
            options: DataManipulation.NewSerializerOptions());

        var jsonObject = JsonSerializer.Deserialize<EncryptedObjectSetting>(
            json: json,
            options: DataManipulation.NewSerializerOptions())!;

        // decrypting verifies the crc of the decrypted data
        var decryptedJsonObject = jsonObject.Decrypt<TestObject>(key: vaultKey);

        Assert.AreEqual(
            expected: expectedObject.ValueA,
            actual: decryptedJsonObject.ValueA);
        Assert.AreEqual(
            expected: expectedObject.ValueB,
            actual: decryptedJsonObject.ValueB);
    }

    [TestMethod]
    public void TestEncryptedString()
    {
        var expectedObjectName = new Faker().Lorem.Word();
        using var vaultKey = NewVaultKey(plainPassword: out var _);
        using var expectedRandomString = NewVaultKey(plainPassword: out var plainString);
        var encryptedObject = EncryptedSecureStringSetting.CreateFromSecureString(
            keyName: expectedObjectName,
            vaultKey: vaultKey,
            secureValue: expectedRandomString);

        // decrypting verifies the crc of the decrypted data
        using var decryptedAsSecureString = encryptedObject.Decrypt(key: vaultKey);

        // ValueType verifies the file type is valid
        Assert.AreEqual(
            expected: expectedRandomString.GetType(),
            actual: encryptedObject.ValueType);
        // verify the string itself
        // get the plain string out of the SecureString
        var stringPointer = Marshal.SecureStringToBSTR(s: decryptedAsSecureString);
        var decryptedPlainString = Marshal.PtrToStringBSTR(ptr: stringPointer);
        Marshal.ZeroFreeBSTR(s: stringPointer);
        Assert.AreEqual(
            expected: plainString,
            actual: decryptedPlainString);

        // ensure data survives serialization and deserialization

        var json = JsonSerializer.Serialize(
            value: encryptedObject,
            options: DataManipulation.NewSerializerOptions());

        var jsonObject = JsonSerializer.Deserialize<EncryptedSecureStringSetting>(
            json: json,
            options: DataManipulation.NewSerializerOptions())!;

        // decrypting verifies the crc of the decrypted data
        var decryptedJsonObject = jsonObject.Decrypt(key: vaultKey);

        var jsonStringPointer = Marshal.SecureStringToBSTR(s: decryptedJsonObject);
        var decryptedInsecureStringPassword = Marshal.PtrToStringBSTR(ptr: jsonStringPointer);
        Marshal.ZeroFreeBSTR(s: stringPointer);
        Assert.AreEqual(
            expected: plainString,
            actual: decryptedPlainString);

        Assert.AreEqual(
            expected: plainString,
            actual: decryptedInsecureStringPassword);
    }

    [TestMethod]
    public void TestNoDuplicateVaultKey()
    {
        using var key1 = NewVaultKey(plainPassword: out var plainPassword1);
        using var key2 = NewVaultKey(plainPassword: out var plainPassword2);
        Assert.AreNotEqual(
            notExpected: plainPassword2,
            actual: plainPassword1);
        Assert.IsFalse(condition: key1.SecureStringEquals(s2: key2));
    }

    [TestMethod]
    public void TestSerializeColorScheme()
    {
        Application.Driver = new FakeDriver();

        // default schema is all zeroes
        var colorScheme = new ColorScheme();
        var prettyScheme = HumanEditableColorScheme.FromColorScheme(colorScheme: colorScheme);
        var bytes = DataManipulation.SerializeObject(value: prettyScheme);

        var deserializedColorScheme = DataManipulation.Deserialize<HumanEditableColorScheme>(data: bytes);
        var deserializedBase = deserializedColorScheme.ToColorScheme();
        Assert.IsTrue(condition: colorScheme.Disabled.Equals(obj: deserializedBase.Disabled));
        Assert.IsTrue(condition: colorScheme.Focus.Equals(obj: deserializedBase.Focus));
        Assert.IsTrue(condition: colorScheme.Normal.Equals(obj: deserializedBase.Normal));
        Assert.IsTrue(condition: colorScheme.HotNormal.Equals(obj: deserializedBase.HotNormal));

        // now change the color and compare it again
        var attr = Attribute.Make(foreground: Color.BrightCyan,
            background: Color.White);

        colorScheme.HotNormal = attr;

        Assert.IsTrue(condition: colorScheme.Disabled.Equals(obj: deserializedBase.Disabled));
        Assert.IsTrue(condition: colorScheme.Focus.Equals(obj: deserializedBase.Focus));
        Assert.IsTrue(condition: colorScheme.Normal.Equals(obj: deserializedBase.Normal));
        Assert.IsFalse(condition: colorScheme.HotNormal.Equals(obj: deserializedBase.HotNormal));
    }
}