using System;
using System.Linq;
using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhantomKit.Helpers;

namespace PhantomKit.Test;

[TestClass]
public class Crc64Test
{
    // TODO: verify CRC64 values!
    // ReSharper disable once StringLiteralTypo
    [DataTestMethod]
    [DataRow(data1: "54686520717569636B2062726F776E20666F78206A756D7073206F76657220746865206C617A7920646F67",
        12758330164905647473U)]
    [DataRow(data1: "0000000000000000000000000000000000000000000000000000000000000000",
        0U)]
    [DataRow(data1: "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF",
        651209950806147072U)]
    [DataRow(data1: "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F",
        9260567444085364859U)]
    public void TestCrc64KnownHashes(string hexString, ulong expected)
    {
        var data = new byte[hexString.Length / 2];
        for (int offset = 0, dataOffset = 0; offset < hexString.Length; offset += 2)
            data[dataOffset++] = Convert.ToByte(
                value: hexString.Substring(
                    startIndex: offset,
                    length: 2),
                fromBase: 16);

        var actual = Crc64.ComputeChecksum(bytes: data);
        Assert.AreEqual(
            expected: expected,
            actual: actual);
    }

    [TestMethod]
    public void TestChecksumBytes()
    {
        var randomBytes = new Faker().Random.Bytes(count: 1000)!;
        // get the expected checksum using the uint version, which is tested more vigorously above
        var expected = Crc64.ComputeChecksum(bytes: randomBytes);
        var expectedBytes = BitConverter.GetBytes(value: expected);
        var actual = Crc64.ComputeChecksumBytes(bytes: randomBytes);
        Assert.IsTrue(
            condition: expectedBytes.SequenceEqual(second: actual));
    }

    [TestMethod]
    public void TestGeneratedTable()
    {
        var generatedTable = Crc64.CreateTable(polynomial: Crc64.Iso3309Polynomial);
        Assert.IsTrue(condition: generatedTable.SequenceEqual(second: Crc64.Iso3309Table));

        var generatedEcmaTable = Crc64.CreateTable(polynomial: Crc64.EcmaPolynomial);
        Assert.IsTrue(condition: generatedEcmaTable.SequenceEqual(second: Crc64.EcmaTable));
    }
}