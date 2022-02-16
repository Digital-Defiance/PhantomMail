using System.Runtime.InteropServices;
using System.Security;

namespace PhantomKit.Helpers;

public static class Utilities
{
    public static readonly string AppSettingsFile = Path.Combine(
        path1: UserDirectory,
        path2: Constants.VaultDirectoryName,
        path3: "appsettings.json");

    public static string UserDirectory =>
        Environment.GetFolderPath(
            folder: Environment.SpecialFolder.UserProfile);

    public static string TypeNameToString(Type type)
    {
        return type.AssemblyQualifiedName ?? type.FullName ?? type.Name;
    }

    public static string GetDefaultVaultFile(string? directoryName = null, string? fileName = null)
    {
        return directoryName is null
            ? Path.Combine(
                path1: UserDirectory,
                path2: Constants.VaultDirectoryName,
                path3: fileName ?? Constants.VaultFileName)
            : Path.Combine(
                path1: directoryName ?? UserDirectory,
                path2: fileName ?? Constants.VaultFileName);
    }

    public static bool SecureStringEquals(this SecureString? s1, SecureString? s2)
    {
        if (s1 is null && s2 is null) return true;

        if (s1 is null || s2 is null) return false;

        var sp1 = Marshal.SecureStringToBSTR(s: s1);
        var sp2 = Marshal.SecureStringToBSTR(s: s2);

        var sv1 = Marshal.PtrToStringBSTR(ptr: sp1);
        var sv2 = Marshal.PtrToStringBSTR(ptr: sp2);

        Marshal.ZeroFreeBSTR(s: sp1);
        Marshal.ZeroFreeBSTR(s: sp2);

        return string.Equals(
            a: sv1,
            b: sv2);
    }

    // public static readonly string AppSettingsFile = Path.Combine(path1: AppDomain.CurrentDomain.BaseDirectory, path2: "appsettings.json");
}