using PhantomKit.Models;

// ReSharper disable MemberCanBePrivate.Global

namespace PhantomKit.Exceptions;

public class AccountNotFoundException : PhantomKitException
{
    public AccountNotFoundException(EncryptedMailAccount encryptedMailAccount) : base(
        message: DefaultMessage(account: encryptedMailAccount))
    {
    }

    public static string DefaultMessage(EncryptedMailAccount account)
    {
        return $"Account ({account.Name}) with id {account.Id} was not found";
    }
}