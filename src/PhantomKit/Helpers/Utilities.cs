namespace PhantomKit.Helpers;

public static class Utilities
{
    public static string UserDirectory =>
        Environment.GetFolderPath(
            folder: Environment.SpecialFolder.UserProfile);

    public static string TypeNameToString(Type type)
    {
        return type.AssemblyQualifiedName ?? type.FullName ?? type.Name;
    }

    public static string GetSettingsFile(string? directoryName = null, string? fileName = null)
    {
        return directoryName is null
            ? Path.Combine(
                path1: UserDirectory,
                path2: Constants.SettingsDirectoryName,
                path3: fileName ?? Constants.SettingsFileName)
            : Path.Combine(
                path1: directoryName ?? UserDirectory,
                path2: fileName ?? Constants.SettingsFileName);
    }
}