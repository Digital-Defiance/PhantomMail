using System.IO;
using System.Security;
using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhantomKit.Models;

namespace PhantomMail.Test;

[TestClass]
public class MailClientTests
{
    private static SecureString NewVaultKey(out string plainPassword)
    {
        var secureString = new SecureString();
        plainPassword = new Faker().Random.AlphaNumeric(length: 16)!;
        foreach (var c in plainPassword) secureString.AppendChar(c: c);
        secureString.MakeReadOnly();
        return secureString;
    }

    [TestMethod]
    public void TestMailClient()
    {
        var vaultKey = NewVaultKey(plainPassword: out var plainPassword);
        var mailSettings = new EncryptableSettingsVault(vaultKey: vaultKey);
        var settingsTemporaryFile = Path.GetTempFileName();
        mailSettings.Save(fileName: settingsTemporaryFile,
            updateConsoleStatus: false);

        var mailSettings2 = EncryptableSettingsVault.Load(vaultKey: vaultKey,
            fileName: settingsTemporaryFile);
    }
}