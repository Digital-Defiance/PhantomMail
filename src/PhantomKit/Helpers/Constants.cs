using System.IO.Compression;
using System.Security.Cryptography;
using PhantomKit.Models.Themes;
using Version = SemanticVersioning.Version;

namespace PhantomKit.Helpers;

public class Constants
{
    // strings/program name
    public const bool UsePrettyConsole = false;
    public const string ProgramName = "PhantomMail";
    public const string LibraryName = "PhantomKit";
    public const string DefaultPassword = "phantom";

    // settings
    public const string VaultFileName = "settings-vault.json";
    public const string VaultDirectoryName = ".phantom-mail";

    /// <summary>
    ///     Size of password salt must be at least 8 bytes.
    /// </summary>
    public const int PasswordSaltSize = 16;

    /// <summary>
    ///     Number of iterations for the key stretching process.
    /// </summary>
    public const int Pbkdf2Iterations = 10000;

    /// <summary>
    ///     Use AES-256 to encrypt the data.
    /// </summary>
    public const int AesKeySize = 256;

    /// <summary>
    ///     Use AES-256-CBC to encrypt the data.
    /// </summary>
    public const CipherMode AesCipherMode = CipherMode.CBC;

    public const PaddingMode AesPaddingMode = PaddingMode.PKCS7;

    /// <summary>
    ///     Compression level for the long string "type" values.
    /// </summary>
    public const CompressionLevel TypeCompressionLevel = CompressionLevel.SmallestSize;

    /// <summary>
    ///     Compression level for the blob fields
    /// </summary>
    public const CompressionLevel DataCompressionLevel = CompressionLevel.Optimal;

    /// <summary>
    ///     Whether saved vaults are pretty printed
    /// </summary>
    public const bool PrettyPrintJson = true;

    /// <summary>
    ///     Default TUI theme
    /// </summary>
    public const string DefaultTheme = DarkHumanEditableTheme.Name;

    public static readonly Version FileVersion = new(major: 1,
        minor: 0,
        patch: 0);
}