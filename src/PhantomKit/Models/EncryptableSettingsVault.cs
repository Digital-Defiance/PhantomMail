using PhantomKit.Exceptions;
using PhantomKit.Helpers;
using PhantomKit.Models.Settings;
using PhantomKit.Models.Themes;
using System.Collections;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.Json;
using System.Text.Json.Serialization;
using Version = SemanticVersioning.Version;

namespace PhantomKit.Models;

/// <summary>
///     Encrypted dictionary. Data is stored in the json file encrypted.
/// </summary>
[Serializable]
public sealed record EncryptableSettingsVault : IDictionary<string, object>, IDisposable
{
    private const string Serilog =
        "{\n  \"Serilog\": {\n    \"MinimumLevel\": \"Debug\",\n    \"WriteTo\": [\n      {\n        \"Name\": \"Console\",\n        \"Args\": {\n          \"outputTemplate\": \"===> {Timestamp:HH:mm:ss.fff zzz} [{Level:w3}] {Message:lj}{NewLine}{Exception}\"\n        }\n      },\n      {\n        \"Name\": \"File\",\n        \"Args\": {\n          \"path\": \"PhantomMail.log\",\n          \"rollingInterval\": \"Day\",\n          \"outputTemplate\": \"===> {Timestamp:HH:mm:ss.fff zzz} [{Level:w3}] {Message:lj}{NewLine}{Exception}\"\n        }\n      }\n    ]\n  }\n}\n";

    public static readonly string[] RequiredSettings =
    {
        nameof(Theme),
    };

    public static readonly string[] RequiredEncryptedSettings =
    {
        nameof(MailAccounts) /* required to be at least an empty collection */,
        nameof(CustomThemes) /* required to be at least an empty collection */,
    };

    private int? _lastSavedHashCode;
    private SecureString? _vaultKey;

    public EncryptableSettingsVault()
    {
        this.EncryptableSettings = new Dictionary<string, object>();
        this.FileVersionString = Constants.FileVersion.ToString();
    }

    public EncryptableSettingsVault(Dictionary<string, object> encryptableSettings)
    {
        this.VerifyRequiredSettingsOrThrow(encryptableSettings: encryptableSettings);

        // expand the virtual array
        this.EncryptableSettings = encryptableSettings;
        this._lastSavedHashCode = null;
        this.FileVersionString = Constants.FileVersion.ToString();
    }

    public EncryptableSettingsVault(EncryptableSettingsVault other)
    {
        var jsonCopy = JsonSerializer.Serialize(value: other.EncryptableSettings,
            options: DataManipulation.NewSerializerOptions());
        this.EncryptableSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(json: jsonCopy,
            options: DataManipulation.NewSerializerOptions()) ?? throw new InvalidOperationException();
        this._lastSavedHashCode = other._lastSavedHashCode;
        this.FileVersionString = other.FileVersionString;
    }

    /// <summary>
    /// </summary>
    /// <param name="vaultKey">Vault key is not kept</param>
    public EncryptableSettingsVault(SecureString vaultKey)
    {
        this.EncryptableSettings = DefaultSettings(vaultKey: vaultKey);
        this.FileVersionString = Constants.FileVersion.ToString();
        this._lastSavedHashCode = this.GetHashCode();
    }

    public SecureString? VaultKey
    {
        get => this._vaultKey;
        set => this.SetVaultKey(vaultKey: value);
    }

    public bool HasChanged
    {
        get
        {
            var hashCode = this.GetHashCode();
            if (this._lastSavedHashCode == null)
                return true;
            return this._lastSavedHashCode != hashCode;
        }
    }

    [JsonIgnore]
    public HumanEditableTheme Theme
    {
        get
        {
            var themeName = this.GetSetting<string>(name: nameof(this.Theme));

            // allow override of builtin themes
            var customThemes = this.CustomThemes();
            if (customThemes.ContainsKey(key: themeName))
                return customThemes[key: themeName];

            // fallback to builtin theme
            return HumanEditableTheme.BuiltinThemes.ContainsKey(key: themeName)
                ? HumanEditableTheme.BuiltinThemes[key: themeName]
                : customThemes.ContainsKey(key: themeName)
                    ? customThemes[key: themeName]
                    : HumanEditableTheme.Themes.Dark;
        }
    }

    [JsonInclude]
    [JsonPropertyName(name: "s")]
    public Dictionary<string, object> EncryptableSettings { get; }

    [JsonInclude]
    [JsonPropertyName(name: "ver")]
    public string FileVersionString { get; private set; }

    public Version FileVersion
    {
        get => Version.Parse(input: this.FileVersionString);
        set => this.FileVersionString = value.ToString();
    }

    public ICollection<string> Keys => ((IDictionary<string, object>)this.EncryptableSettings).Keys;

    public ICollection<object> Values
        => ((IDictionary<string, object>)this.EncryptableSettings).Values;

    public void Add(string key, object value)
    {
        if (value is string plainStringSetting ||
            value is EncryptableObjectSetting encryptableObjectSetting)
            this.EncryptableSettings.Add(key: key,
                value: value);
    }

    public bool ContainsKey(string key)
    {
        return this.EncryptableSettings.ContainsKey(key: key);
    }

    public bool Remove(string key)
    {
        return this.EncryptableSettings.Remove(key: key);
    }

    public bool TryGetValue(string key, out object value)
    {
        return this.EncryptableSettings.TryGetValue(key: key,
            value: out value!);
    }

    public object this[string key]
    {
        get => ((IDictionary<string, object>)this.EncryptableSettings)[key: key];
        set
        {
            switch (value)
            {
                case JsonElement { ValueKind: JsonValueKind.String } jsonElement:
                    this.EncryptableSettings[key: key] = jsonElement.GetString()!;
                    return;
                case JsonElement { ValueKind: JsonValueKind.Object } jsonElement:
                    var testEncryptedObject = jsonElement.Deserialize<EncryptedObjectSetting>();
                    if (testEncryptedObject is not null)
                    {
                        this.EncryptableSettings[key: key] = testEncryptedObject;
                        return;
                    }

                    var testEncryptableObject = jsonElement.Deserialize<EncryptableObjectSetting>();
                    if (testEncryptableObject is not null)
                    {
                        this.EncryptableSettings[key: key] = testEncryptableObject;
                        return;
                    }

                    break;
                case string plainStringSetting:
                    this.EncryptableSettings[key: key] = plainStringSetting;
                    return;
                case EncryptedObjectSetting encryptedObjectSetting:
                    this.EncryptableSettings[key: key] = encryptedObjectSetting;
                    return;
                case EncryptableObjectSetting encryptableObjectSetting:
                    this.EncryptableSettings[key: key] = encryptableObjectSetting;
                    return;
            }

            throw new NotSupportedException();
        }
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return this.EncryptableSettings.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)this.EncryptableSettings).GetEnumerator();
    }

    public void Add(KeyValuePair<string, object> item)
    {
        if (item.Value is string plainStringSetting ||
            item.Value is EncryptableObjectSetting encryptableObjectSetting)
            this.EncryptableSettings.Add(key: item.Key,
                value: item.Value);
        throw new NotSupportedException();
    }

    public void Clear()
    {
        this.EncryptableSettings.Clear();
    }

    public bool Contains(KeyValuePair<string, object> item)
    {
        return this.EncryptableSettings.Contains(value: item) &&
               this.EncryptableSettings[key: item.Key].Equals(obj: item.Value);
    }

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
        if ((uint)arrayIndex > (uint)array.Length)
            throw new IndexOutOfRangeException(
                message: "The arrayIndex is greater than the array.Length");

        if (array.Length - arrayIndex < this.EncryptableSettings.Count)
            throw new ArgumentException(
                message:
                "The number of elements in the source ICollection<T> is greater than the available space from arrayIndex to the end of the destination array.");

        foreach (var (key, value) in this.EncryptableSettings)
            array[arrayIndex++] = new KeyValuePair<string, object>(key: key,
                value: value);
    }

    public bool Remove(KeyValuePair<string, object> item)
    {
        return this.EncryptableSettings.Remove(key: item.Key);
    }

    public int Count => this.EncryptableSettings.Count;
    public bool IsReadOnly => false;

    public void Dispose()
    {
        this._vaultKey?.Dispose();
    }

    private void VerifyRequiredSettingsOrThrow(Dictionary<string, object> encryptableSettings)
    {
        foreach (var requiredSetting in RequiredSettings)
            if (!encryptableSettings.ContainsKey(key: requiredSetting))
                throw new InvalidVaultDataException(paramName: requiredSetting,
                    paramValue: "<missing>");

        foreach (var requiredEncryptedSetting in RequiredEncryptedSettings)
            if (!encryptableSettings.ContainsKey(key: requiredEncryptedSetting))
                throw new InvalidVaultDataException(paramName: requiredEncryptedSetting,
                    paramValue: "<missing, encrypted>");
    }

    public IDictionary<string, object> SetItem(string key, object value)
    {
        this.EncryptableSettings[key: key] = value switch
        {
            string plainStringSetting => plainStringSetting,
            EncryptableObjectSetting encryptableObjectSetting => encryptableObjectSetting,
            _ => this.EncryptableSettings[key: key],
        };

        throw new NotSupportedException();
    }

    public IDictionary<string, object> SetItems(
        IEnumerable<KeyValuePair<string, object>> items)
    {
        foreach (var (key, value) in items)
            this.EncryptableSettings[key: key] = value switch
            {
                string plainStringSetting => plainStringSetting,
                EncryptableObjectSetting encryptableObjectSetting => encryptableObjectSetting,
                _ => throw new NotSupportedException(),
            };
        return this.EncryptableSettings;
    }

    public EncryptableSettingsVault UpdateVaultKey(SecureString oldKey, SecureString newKey)
    {
        // create a new settings dictionary which we will copy over from the old settings as we re-encrypt
        var newDictionary =
            new Dictionary<string, object>(
                dictionary: this.EncryptableSettings);

        foreach (var (keyName, setting) in this.EncryptableSettings)
            switch (setting)
            {
                case EncryptedObjectSetting encryptedSetting:
                    // decrypt with the old key
                    // re-create with the new key
                    newDictionary.Add(
                        key: keyName,
                        value:
                        EncryptedObjectSetting
                            .CreateEncryptedFromBytes( /* this constructor skips deserialization/re-serialization */
                                unencryptedObjectData: EncryptedObjectSetting.DecryptAsBytes(key: oldKey,
                                    crc32: encryptedSetting.Crc32,
                                    dataLength: encryptedSetting.DataLength,
                                    iv: encryptedSetting.Iv,
                                    passwordSalt: encryptedSetting.PasswordSalt,
                                    encryptedBytes: encryptedSetting.ValueBytes),
                                originalLength: encryptedSetting.DataLength,
                                vaultKey: newKey,
                                valueType: encryptedSetting.ValueType));
                    break;
                case EncryptableObjectSetting encryptableObjectSetting:
                    newDictionary.Add(key: keyName,
                        value: encryptableObjectSetting);
                    break;
                case string plainSetting:
                    newDictionary.Add(
                        key: keyName,
                        value: plainSetting);
                    break;
                default:
                    throw new NotSupportedException();
            }

        // return a new copy of the settings collection with the new encrypted settings
        return new EncryptableSettingsVault(encryptableSettings: newDictionary);
    }

    public bool ValidateKey(SecureString key, out string failedKey, out Exception? exception)
    {
        foreach (var (keyName, setting) in this.EncryptableSettings)
            switch (setting)
            {
                case EncryptedObjectSetting { Encrypted: true } encryptedSetting:
                    try
                    {
                        EncryptedObjectSetting.DecryptAsBytes(key: key,
                            crc32: encryptedSetting.Crc32,
                            iv: encryptedSetting.Iv,
                            passwordSalt: encryptedSetting.PasswordSalt,
                            dataLength: encryptedSetting.DataLength,
                            encryptedBytes: encryptedSetting.ValueBytes);
                    }
                    catch (PhantomKitException pkEx)
                    {
                        failedKey = keyName;
                        exception = pkEx;
                        return false;
                    }
                    catch (Exception e)
                    {
                        failedKey = keyName;
                        exception = e;
                        return false;
                    }

                    break;
                case EncryptedObjectSetting { Encrypted: false } fauxEncryptedSetting:
                case EncryptableObjectSetting encryptableObjectSetting:
                    // base encryptable object settings have no key to check
                    continue;
                case string plainSetting:
                    // plain settings have no key to check
                    continue;
            }

        failedKey = string.Empty;
        exception = null;
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(value: this.FileVersion.GetHashCode());
        foreach (var (keyName, setting) in this.EncryptableSettings)
        {
            hashCode.Add(value: keyName.GetHashCode());
            hashCode.Add(value: setting.GetHashCode());
        }

        return hashCode.ToHashCode();
    }

    public ImmutableDictionary<string, HumanEditableTheme> CustomThemes()
    {
        if (!this.HasSetting(name: nameof(this.CustomThemes)))
        {
            var newDict = new Dictionary<string, HumanEditableTheme>().ToImmutableDictionary();
            this.EncryptableSettings[key: nameof(this.CustomThemes)] = EncryptableObjectSetting.Create(value: newDict);
            return newDict;
        }

        var encryptableObjectSetting = this.GetSetting<EncryptableObjectSetting>(name: nameof(this.CustomThemes));
        return (encryptableObjectSetting.DataLength == 0
                ? new Dictionary<string, HumanEditableTheme>()
                : encryptableObjectSetting.Object<Dictionary<string, HumanEditableTheme>>())
            .ToImmutableDictionary();
    }

    public static Dictionary<string, object> DefaultSettings(SecureString vaultKey)
    {
        return new Dictionary<string, object>
        {
            [key: nameof(Theme)] = Constants.DefaultTheme,
            [key: nameof(MailAccounts)] = EncryptedObjectSetting.CreateEncryptedFromBytes(
                vaultKey: vaultKey,
                unencryptedObjectData: Array.Empty<byte>(),
                originalLength: 0,
                valueType: typeof(IEnumerable<EncryptedMailAccount>)),
            [key: nameof(CustomThemes)] = EncryptableObjectSetting.CreateFromBytes(
                unencryptedObjectData: Array.Empty<byte>(),
                originalLength: 0,
                valueType: typeof(Dictionary<string, HumanEditableTheme>)),
            [key: nameof(Serilog)] = JsonSerializer.Deserialize<JsonDocument>(
                json: File.ReadAllText(path: Utilities.AppSettingsFile),
                options: DataManipulation.NewSerializerOptions()) ?? throw new InvalidOperationException(),
        };
    }

    public bool HasSetting(string name)
    {
        if (!this.EncryptableSettings.ContainsKey(key: name)) return false;

        var setting = this.EncryptableSettings[key: name];
        if (setting is string) return true;
        return setting is EncryptableObjectSetting { Encrypted: false };
    }

    public bool HasEncryptedSetting(string name)
    {
        if (!this.EncryptableSettings.ContainsKey(key: name)) return false;
        var setting = this.EncryptableSettings[key: name];
        return setting is EncryptedObjectSetting { Encrypted: true };
    }

    public object GetSetting(string name)
    {
        if (!this.EncryptableSettings.ContainsKey(key: name))
            throw new InvalidVaultDataException(
                paramName: name,
                paramValue: null);

        var setting = this.EncryptableSettings[key: name];

        return setting switch
        {
            string plainStringSetting => plainStringSetting,
            EncryptedObjectSetting encryptedObjectSetting => throw new ArgumentException(paramName: name,
                message: "Request setting is encrypted"),
            EncryptableObjectSetting encryptableObjectSetting when encryptableObjectSetting.Encrypted => throw new
                ArgumentException(paramName: name,
                    message: "Request setting is encrypted"),
            EncryptableObjectSetting encryptableObjectSetting => encryptableObjectSetting.Object(),
            _ => throw new NotSupportedException(),
        };
    }

    public T GetSetting<T>(string name)
    {
        if (!this.EncryptableSettings.ContainsKey(key: name))
            throw new InvalidVaultDataException(
                paramName: name,
                paramValue: null);

        var setting = this.EncryptableSettings[key: name];
        if (setting is T settingT)
            return settingT;
        throw new InvalidVaultDataException(
            paramName: name,
            paramValue: $"Setting was not the expected type. Expected {typeof(T).Name}, got {setting.GetType().Name}");
    }

    public EncryptedObjectSetting GetEncryptedSetting(string name)
    {
        var setting = this.EncryptableSettings[key: name];
        if (setting is EncryptedObjectSetting encryptedObjectSetting)
            return encryptedObjectSetting;
        throw new ArgumentException(
            paramName: name,
            message: "Request setting is not encrypted");
    }

    /// <summary>
    ///     Write the settings to the specified file.
    /// </summary>
    /// <param name="fileName"></param>
    public void Save(string? fileName = null)
    {
        fileName ??= Utilities.GetDefaultVaultFile();
        var directory = Path.GetDirectoryName(path: fileName)!;
        if (!Directory.Exists(path: directory))
            Directory.CreateDirectory(path: directory);

        File.WriteAllText(
            path: fileName,
            contents: JsonSerializer.Serialize(
                value: this,
                options: DataManipulation.NewSerializerOptions()));

        this._lastSavedHashCode = this.GetHashCode();
    }

    public static EncryptableSettingsVault Load(SecureString vaultKey, string? fileName = null)
    {
        fileName ??= Utilities.GetDefaultVaultFile();
        if (!File.Exists(path: fileName))
            throw new VaultFileNotFoundException(fileName: fileName);

        var settingsVault = JsonSerializer.Deserialize<EncryptableSettingsVault>(
            json: File.ReadAllText(path: fileName),
            options: DataManipulation.NewSerializerOptions())!;
        settingsVault.SetVaultKey(vaultKey: vaultKey);

        // verify that the vault key is correct
        if (settingsVault.ValidateKey(
                key: vaultKey,
                failedKey: out var failedKey,
                exception: out var thrownException))
            return settingsVault;

        throw new VaultKeyIncorrectException(
            paramName: failedKey,
            innerException: thrownException);
    }

    public void SetVaultKey(SecureString? vaultKey)
    {
        if (this._vaultKey is not null)
        {
            this._vaultKey.Dispose();
            this._vaultKey = null;
        }

        if (vaultKey is null)
            return;

        var stringPointer = Marshal.SecureStringToBSTR(s: vaultKey);
        var secureValueString = Marshal.PtrToStringBSTR(ptr: stringPointer);
        Marshal.ZeroFreeBSTR(s: stringPointer);

        var newSecureString = new SecureString();
        foreach (var c in secureValueString) newSecureString.AppendChar(c: c);
        newSecureString.MakeReadOnly();
        this._vaultKey = newSecureString;
    }

    public void ClearVaultKey()
    {
        if (this._vaultKey is null) return;
        this._vaultKey.Dispose();
        this._vaultKey = null;
    }

    public object GetEncryptedSettingAndDecrypt(string key, SecureString? vaultKey = null)
    {
        if (vaultKey is null && this._vaultKey is null)
            throw new VaultKeyNotSetException();

        var encryptedSetting = this.GetEncryptedSetting(name: key);
        return encryptedSetting.Decrypt(key: vaultKey ?? this._vaultKey!);
    }

    public MailAccounts MailAccounts(SecureString? vaultKey = null)
    {
        if (vaultKey is null && this._vaultKey is null)
            throw new VaultKeyNotSetException();

        if (!this.HasEncryptedSetting(name: nameof(this.MailAccounts)))
            return new MailAccounts();

        var setting = this.GetEncryptedSetting(name: nameof(this.MailAccounts));
        return setting.DataLength == 0
            ? new MailAccounts()
            : setting.Decrypt<MailAccounts>(key: vaultKey ?? this._vaultKey!);
    }
}