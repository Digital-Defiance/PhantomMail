using PhantomKit.Models;
using PhantomMail.CLI.Commands;
using Terminal.Gui;

namespace PhantomMail.CLI.Views;

public class AccountMailboxView : View
{
    public static void View(in EncryptedMailAccount encryptedMailAccount, in PhantomMailGuiCommand guiCommand)
    {
        var connection = guiCommand.ConnectAccount(account: encryptedMailAccount);
    }
}