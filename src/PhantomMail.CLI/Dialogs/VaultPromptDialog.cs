using System.Security;
using NStack;
using Terminal.Gui;

namespace PhantomMail.CLI.Dialogs;

public class VaultPromptDialog : Dialog
{
    public const string TitleString = "Vault Key";
    private const int DefaultWidth = 60;
    private const int MesssageBoxHeight = 6; // (top + top padding + password row + buttons + bottom)

    private static readonly Button PasswordDialogOkButton = new(
        text: ustring.Make(str: "OK"),
        is_default: true);

    private static readonly Button PasswordDialogCancelButton = new(
        text: ustring.Make(str: "Cancel"),
        is_default: false);

    private static readonly TextField VaultPassword = new(text: "")
    {
        Secret = true,
        X = 2,
        Y = 1,
        Width = DefaultWidth - 6,
    };

    private Label _passwordLabel;

    public VaultPromptDialog(int width = DefaultWidth, int height = MesssageBoxHeight, bool verify = false) : base(
        title: ustring.Make(str: TitleString),
        width: width,
        height: height,
        buttons: new[] {PasswordDialogOkButton, PasswordDialogCancelButton})
    {
        this._passwordLabel = GetPasswordLabel(verify: verify);
        this.SetPasswordLabel(verify: verify);
        PasswordDialogOkButton.Clicked += this.OnPasswordDialogOkClicked;
        PasswordDialogCancelButton.Clicked += this.OnPasswordDialogCancelClicked;
        base.Add(view: this._passwordLabel);
        base.Add(view: VaultPassword);
        VaultPassword.SetFocus();
    }

    public SecureString? VaultKeySecureString { get; private set; }

    private static ustring PasswordLabelString(bool verify)
    {
        return ustring.Make(str: verify ? "Verify:" : "Password:");
    }

    public static Label GetPasswordLabel(bool verify)
    {
        var labelString = PasswordLabelString(verify: verify);
        return new Label(text: labelString)
        {
            X = 2,
            Y = 0,
            Width = labelString.RuneCount,
        };
    }

    public void SetPasswordLabel(bool verify)
    {
        this._passwordLabel = GetPasswordLabel(verify: verify);
    }

    private void OnPasswordDialogOkClicked()
    {
        var insecurePassword = VaultPassword.Text.ToString()!;
        VaultPassword.Text = ustring.Make(str: string.Empty);
        var result = new SecureString();
        foreach (var c in insecurePassword) result.AppendChar(c: c);
        result.MakeReadOnly();
        this.VaultKeySecureString = result;
        Application.RequestStop();
    }

    private void OnPasswordDialogCancelClicked()
    {
        VaultPassword.Text = ustring.Make(str: string.Empty);
        this.VaultKeySecureString = null;
        Application.RequestStop();
    }
}