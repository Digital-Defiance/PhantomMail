using System.Runtime.InteropServices;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using PhantomKit.Enumerations;
using PhantomKit.Exceptions;
using PhantomKit.Models;

// ReSharper disable MemberCanBePrivate.Global

namespace PhantomKit.Helpers;

/// <summary>
///     Core PhantomMail client program
///     Part Trois
/// </summary>
public static class MailUtilities
{
    /// <summary>
    ///     Connect to an SMTP (send) server.
    /// </summary>
    /// <param name="mailAccount"></param>
    /// <returns></returns>
    /// <exception cref="InvalidAccountTypeException"></exception>
    private static SmtpClient ConnectSmtp(UnlockedMailAccount mailAccount)
    {
        if (mailAccount.Type != AccountType.Smtp) throw new InvalidAccountTypeException(accountType: mailAccount.Type);

        var smtpClient = new SmtpClient();
        smtpClient.Connect(host: mailAccount.Host,
            port: mailAccount.PortNumber,
            useSsl: mailAccount.UseSsl);

        if (mailAccount.Username.Length == 0) return smtpClient;

        var stringPointer = Marshal.SecureStringToBSTR(s: mailAccount.Username);
        var usernameString = Marshal.PtrToStringBSTR(ptr: stringPointer);
        Marshal.ZeroFreeBSTR(s: stringPointer);

        stringPointer = Marshal.SecureStringToBSTR(s: mailAccount.Password);
        var passwordString = Marshal.PtrToStringBSTR(ptr: stringPointer);
        Marshal.ZeroFreeBSTR(s: stringPointer);

        smtpClient.Authenticate(
            userName: usernameString,
            password: passwordString);

        return smtpClient;
    }

    /// <summary>
    ///     Connect to an IMAP (receive) server.
    /// </summary>
    /// <param name="mailAccount"></param>
    /// <returns></returns>
    /// <exception cref="InvalidAccountTypeException"></exception>
    private static ImapClient ConnectImap(UnlockedMailAccount mailAccount)
    {
        if (mailAccount.Type != AccountType.Imap) throw new InvalidAccountTypeException(accountType: mailAccount.Type);

        var imapClient = new ImapClient();
        imapClient.Connect(host: mailAccount.Host,
            port: mailAccount.PortNumber,
            useSsl: mailAccount.UseSsl);
        if (mailAccount.Username.Length == 0) return imapClient;

        var stringPointer = Marshal.SecureStringToBSTR(s: mailAccount.Username);
        var usernameString = Marshal.PtrToStringBSTR(ptr: stringPointer);
        Marshal.ZeroFreeBSTR(s: stringPointer);

        stringPointer = Marshal.SecureStringToBSTR(s: mailAccount.Password);
        var passwordString = Marshal.PtrToStringBSTR(ptr: stringPointer);
        Marshal.ZeroFreeBSTR(s: stringPointer);

        imapClient.Authenticate(
            userName: usernameString,
            password: passwordString);

        return imapClient;
    }

    /// <summary>
    ///     Connect to a POP3 (receive) server.
    /// </summary>
    /// <param name="mailAccount"></param>
    /// <returns></returns>
    /// <exception cref="InvalidAccountTypeException"></exception>
    private static Pop3Client ConnectPop3(UnlockedMailAccount mailAccount)
    {
        if (mailAccount.Type != AccountType.Pop3) throw new InvalidAccountTypeException(accountType: mailAccount.Type);

        var pop3Client = new Pop3Client();
        pop3Client.Connect(host: mailAccount.Host,
            port: mailAccount.PortNumber,
            useSsl: mailAccount.UseSsl);
        if (mailAccount.Username.Length == 0) return pop3Client;

        var stringPointer = Marshal.SecureStringToBSTR(s: mailAccount.Username);
        var usernameString = Marshal.PtrToStringBSTR(ptr: stringPointer);
        Marshal.ZeroFreeBSTR(s: stringPointer);

        stringPointer = Marshal.SecureStringToBSTR(s: mailAccount.Password);
        var passwordString = Marshal.PtrToStringBSTR(ptr: stringPointer);
        Marshal.ZeroFreeBSTR(s: stringPointer);

        pop3Client.Authenticate(
            userName: usernameString,
            password: passwordString);

        return pop3Client;
    }

    /// <summary>
    ///     Connect a mail service using a EncryptedMailAccount
    /// </summary>
    /// <param name="unlockedMailAccount"></param>
    /// <returns></returns>
    /// <exception cref="InvalidAccountTypeException"></exception>
    public static IMailService ConnectMailService(UnlockedMailAccount unlockedMailAccount)
    {
        return unlockedMailAccount.Type switch
        {
            AccountType.Smtp => ConnectSmtp(mailAccount: unlockedMailAccount),
            AccountType.Imap => ConnectImap(mailAccount: unlockedMailAccount),
            AccountType.Pop3 => ConnectPop3(mailAccount: unlockedMailAccount),
            _ => throw new InvalidAccountTypeException(accountType: unlockedMailAccount.Type),
        };
    }
}