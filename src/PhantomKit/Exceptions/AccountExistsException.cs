using PhantomKit.Models;

// ReSharper disable MemberCanBePrivate.Global

namespace PhantomKit.Exceptions;

public class AccountExistsException : PhantomKitException
{
    public AccountExistsException(EncryptedMailAccount encryptedMailAccount) : base(
        message: DefaultMessage(accountId: encryptedMailAccount.Id))
    {
    }

    public static string DefaultMessage(Guid accountId)
    {
        return $"Account with id {accountId} already exists";
    }
}