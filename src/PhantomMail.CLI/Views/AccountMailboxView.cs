using PhantomKit.Models;
using Terminal.Gui;

namespace PhantomMail.Views;

public class AccountMailboxView : View
{
    public static void View(in EncryptedMailAccount encryptedMailAccount, in GuiContext guiContext)
    {
        var connection = guiContext.ConnectAccount(account: encryptedMailAccount);
    }
}