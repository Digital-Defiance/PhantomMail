using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhantomKit.Helpers;
using PhantomKit.Models;
using Terminal.Gui;

namespace PhantomKit.Test;

[TestClass]
public class UtilitiesTests
{
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