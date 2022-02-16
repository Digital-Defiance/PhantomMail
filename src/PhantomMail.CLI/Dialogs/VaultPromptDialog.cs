using System.Security;
using NStack;
using Terminal.Gui;

namespace PhantomMail.CLI.Dialogs;

public class VaultPromptDialog : Dialog
{
    private const int DefaultWidth = 60;
    private const int MesssageBoxHeight = 6; // (top + top padding + password row + buttons + bottom)

    private static readonly Button PasswordDialogOkButton = new(
        text: ustring.Make(str: "OK"),
        is_default: true);

    private static readonly Button PasswordDialogCancelButton = new(
        text: ustring.Make(str: "Cancel"),
        is_default: false);

    private readonly TextField _vaultPasswordText;

    private readonly TextField? _vaultPasswordVerifyText;


    public VaultPromptDialog(int width = DefaultWidth, int height = MesssageBoxHeight, bool withVerify = false) : base(
        title: TitleString(withVerify: withVerify),
        width: width,
        height: withVerify ? height + 1 : height,
        buttons: new[] {PasswordDialogOkButton, PasswordDialogCancelButton})
    {
        var labelString = PasswordLabelString(verify: false);
        var verifyLabelString = PasswordLabelString(verify: true);
        var maxLabelWidth = Math.Max(val1: labelString.RuneCount,
            val2: verifyLabelString.RuneCount);

        var passwordLabel = new Label(text: labelString)
        {
            X = 2,
            Y = 1,
            Width = maxLabelWidth,
        };

        this._vaultPasswordText = new TextField(text: "")
        {
            Secret = true,
            X = Pos.Right(view: passwordLabel),
            Y = 1,
            Width = Dim.Fill() - 2,
            TabIndex = 0,
        };

        base.Add(view: passwordLabel);

        if (withVerify)
        {
            var passwordVerifyLabel = new Label(text: verifyLabelString)
            {
                X = 2,
                Y = Pos.Bottom(view: passwordLabel),
                Width = maxLabelWidth,
            };
            this._vaultPasswordVerifyText = new TextField(text: "")
            {
                Secret = true,
                X = Pos.Right(view: passwordVerifyLabel),
                Y = Pos.Bottom(view: this._vaultPasswordText),
                Width = Dim.Fill() - 2,
                TabIndex = 1,
            };
            base.Add(view: passwordVerifyLabel);
            base.Add(view: this._vaultPasswordVerifyText);
        }

        base.Add(view: this._vaultPasswordText);
        PasswordDialogOkButton.Clicked += this.OnPasswordDialogOkClicked;
        PasswordDialogCancelButton.Clicked += this.OnPasswordDialogCancelClicked;
        this._vaultPasswordText.SetFocus();
    }

    public SecureString? VaultKeySecureString { get; private set; }

    public static ustring TitleString(bool withVerify)
    {
        return ustring.Make(str: withVerify ? "New Vault Key" : "Vault Key");
    }

    public void ClearSecureString(bool dispose = true)
    {
        if (dispose)
            this.VaultKeySecureString?.Dispose();
        this.VaultKeySecureString = null;
    }

    private static ustring PasswordLabelString(bool verify)
    {
        return ustring.Make(str: verify ? "Verify: " : "Password: ");
    }

    private void OnPasswordDialogOkClicked()
    {
        var insecurePassword = this._vaultPasswordText.Text.ToString()!;
        if (this._vaultPasswordVerifyText is not null)
        {
            var insecurePasswordVerify = this._vaultPasswordVerifyText.Text.ToString()!;
            if (insecurePassword != insecurePasswordVerify)
            {
                var message = ustring.Make(str: "The passwords you entered do not match. Please try again.");
                MessageBox.ErrorQuery(
                    title: ustring.Make(str: "Passwords do not match"),
                    message: ustring.Make(str: "The passwords you entered do not match. Please try again."),
                    width: message.RuneCount,
                    height: 4);
                return;
            }
        }

        this._vaultPasswordText.Text = ustring.Make(str: string.Empty);
        var result = new SecureString();
        foreach (var c in insecurePassword) result.AppendChar(c: c);
        result.MakeReadOnly();
        this.VaultKeySecureString?.Dispose();
        this.VaultKeySecureString = result;
        Application.RequestStop();
    }

    private void OnPasswordDialogCancelClicked()
    {
        this._vaultPasswordText.Text = ustring.Make(str: string.Empty);
        this.ClearSecureString();
        Application.RequestStop();
    }
}