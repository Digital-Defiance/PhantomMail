using System.Security;
using System.Text.Json.Serialization;
using PhantomKit.Enumerations;
using PhantomKit.Models.Settings;

namespace PhantomKit.Models;

/// <summary>
///     Contains encrypted data for a mail account
/// </summary>
[Serializable]
public record EncryptedMailAccount
{
    /// <summary>
    /// </summary>
    /// <param name="vaultKey"></param>
    /// <param name="description"></param>
    /// <param name="host"></param>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="password"></param>
    /// <param name="portNumber"></param>
    /// <param name="type"></param>
    /// <param name="useDefaultCredentials"></param>
    /// <param name="username"></param>
    /// <param name="useSsl"></param>
    public EncryptedMailAccount(SecureString vaultKey, string description, string host, Guid id, string name,
        SecureString password, int portNumber, AccountType type, bool useDefaultCredentials, SecureString username,
        bool useSsl)
    {
        this.Description = description;
        this.Host = host;
        this.Id = id;
        this.Name = name;
        this.Password = EncryptedSecureStringSetting.CreateFromSecureString(
            keyName: nameof(this.Password),
            vaultKey: vaultKey,
            secureValue: password);
        this.PortNumber = portNumber;
        this.Type = type;
        this.UseDefaultCredentials = useDefaultCredentials;
        this.Username = EncryptedSecureStringSetting.CreateFromSecureString(
            keyName: nameof(this.Username),
            vaultKey: vaultKey,
            secureValue: username);
        this.UseSsl = useSsl;
    }

    [JsonInclude]
    [JsonPropertyName(name: "d")]
    public string Description { get; private set; }

    [JsonInclude]
    [JsonPropertyName(name: "h")]
    public string Host { get; private set; }

    [JsonInclude]
    [JsonPropertyName(name: "i")]
    public Guid Id { get; private set; }

    [JsonInclude]
    [JsonPropertyName(name: "n")]
    public string Name { get; private set; }

    [JsonInclude]
    [JsonPropertyName(name: "p")]
    public EncryptedSecureStringSetting Password { get; private set; }

    [JsonInclude]
    [JsonPropertyName(name: "pn")]
    public int PortNumber { get; private set; }

    [JsonInclude]
    [JsonPropertyName(name: "t")]
    public AccountType Type { get; private set; }

    [JsonInclude]
    [JsonPropertyName(name: "d")]
    public bool UseDefaultCredentials { get; private set; }

    [JsonInclude]
    [JsonPropertyName(name: "u")]
    public EncryptedSecureStringSetting Username { get; private set; }

    [JsonInclude]
    [JsonPropertyName(name: "s")]
    public bool UseSsl { get; private set; }

    public SecureString GetPassword(SecureString vaultKey)
    {
        return this.Password.Decrypt(key: vaultKey);
    }

    public SecureString GetUsername(SecureString vaultKey)
    {
        return this.Username.Decrypt(key: vaultKey);
    }

    public UnlockedMailAccount Unlock(SecureString vaultKey)
    {
        return new UnlockedMailAccount(
            encryptedMailAccount: this,
            vaultKey: vaultKey);
    }
}