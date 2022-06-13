using System.Collections;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PhantomKit.Models;

/// <summary>
///     Collection of mail accounts
/// </summary>
public record MailAccounts : IEnumerable<EncryptedMailAccount>
{
    /// <summary>
    /// </summary>
    /// <param name="accounts"></param>
    public MailAccounts(IEnumerable<EncryptedMailAccount> accounts)
    {
        this.Accounts = accounts;
    }

    /// <summary>
    /// </summary>
    public MailAccounts()
    {
        this.Accounts = Array.Empty<EncryptedMailAccount>();
    }

    /// <summary>
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    public MailAccounts(SerializationInfo info, StreamingContext context)
    {
        this.Accounts = (IEnumerable<EncryptedMailAccount>)info.GetValue(name: "Accounts",
            type: typeof(IEnumerable<EncryptedMailAccount>))!;
    }

    [JsonInclude]
    [JsonPropertyName(name: "a")]
    public IEnumerable<EncryptedMailAccount> Accounts { get; }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.Accounts.GetEnumerator();
    }

    public IEnumerator<EncryptedMailAccount> GetEnumerator()
    {
        return this.Accounts.GetEnumerator();
    }

    public override int GetHashCode()
    {
        return this.Accounts.GetHashCode();
    }
}