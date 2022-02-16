namespace PhantomKit.Helpers;

public class Crc64Ecma : Crc64
{
    public Crc64Ecma()
        : base(polynomial: EcmaPolynomial,
            seed: DefaultSeed,
            table: EcmaTable)
    {
    }
}