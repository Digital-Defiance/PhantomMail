namespace PhantomKit.Helpers;

public class Crc64Iso : Crc64
{
    public Crc64Iso()
        : base(polynomial: Iso3309Polynomial,
            seed: DefaultSeed,
            table: Iso3309Table)
    {
    }
}