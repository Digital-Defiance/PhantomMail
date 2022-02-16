using PhantomKit.Enumerations;

// ReSharper disable MemberCanBePrivate.Global

namespace PhantomKit.Exceptions;

public class InvalidAccountTypeException : PhantomKitException
{
    public InvalidAccountTypeException(AccountType accountType) : base(
        message: DefaultMessage(accountType: accountType))
    {
    }

    public static string DefaultMessage(AccountType accountType)
    {
        return $"Invalid account type: {accountType}";
    }
}