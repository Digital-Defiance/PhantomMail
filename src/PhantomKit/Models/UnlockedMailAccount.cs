using System.Security;
using System.Text.Json.Serialization;

// ReSharper disable MemberCanBePrivate.Global

namespace PhantomKit.Models;

/// <summary>
///     A mail account that has been decrypted into an UnlockedMailAccount
/// </summary>
public sealed record UnlockedMailAccount : EncryptedMailAccount, IDisposable
{
    [JsonIgnore] public new readonly SecureString Password;

    [JsonIgnore] public new readonly SecureString Username;

    public UnlockedMailAccount(EncryptedMailAccount encryptedMailAccount, SecureString vaultKey) : base(
        original: encryptedMailAccount)
    {
        this.Password = encryptedMailAccount.Password.Decrypt(key: vaultKey);
        this.Username = encryptedMailAccount.Username.Decrypt(key: vaultKey);
    }

    public void Dispose()
    {
        this.Password.Dispose();
        this.Username.Dispose();
    }
}