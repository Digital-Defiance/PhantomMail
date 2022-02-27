using System.Security;
using NStack;
using Terminal.Gui;

namespace PhantomMail.Windows;

/// <summary>
///     MessageBox displays a modal message to the user, with a title, a message and a series of options that the user can
///     choose from.
/// </summary>
/// <para>
///     The difference between the <see cref="Query(ustring, ustring, ustring[])" /> and
///     <see cref="ErrorQuery(ustring, ustring, ustring[])" />
///     method is the default set of colors used for the message box.
/// </para>
/// <para>
///     The following example pops up a <see cref="MessageBox" /> with the specified title and text, plus two
///     <see cref="Button" />s.
///     The value -1 is returned when the user cancels the <see cref="MessageBox" /> by pressing the ESC key.
/// </para>
/// <example>
///     <code lang="c#">
/// var n = MessageBox.Query ("Quit Demo", "Are you sure you want to quit this demo?", "Yes", "No");
/// if (n == 0)
///    quit = true;
/// else
///    quit = false;
/// </code>
/// </example>
public class VaultPrompt : View
{
    private const int DefaultWidth = 50;
    private const int MesssageBoxHeight = 5; // (top + top padding + password row + buttons + bottom)

    private readonly Button[] _buttons;
    private readonly Dialog _passwordDialog;
    private readonly Button _passwordDialogCancelButton;
    private readonly Button _passwordDialogOkButton;
    private readonly TextField _vaultPassword;

    public VaultPrompt(int width, int height, Border? border = null)
    {
        this._passwordDialogOkButton = new Button(text: ustring.Make(str: "OK"));
        this._passwordDialogCancelButton = new Button(text: ustring.Make(str: "Cancel"),
            is_default: true);
        this._buttons = new[] {this._passwordDialogOkButton, this._passwordDialogCancelButton};

        // Create button array for Dialog;
        var buttonWidth = this._passwordDialogOkButton.Bounds.Width + this._passwordDialogCancelButton.Bounds.Width + 1;

        var passwordLabel = new Label(text: "Password: ")
        {
            X = 3,
            Y = 2,
        };
        this._vaultPassword = new TextField(text: "")
        {
            Secret = true,
            X = Pos.Left(view: passwordLabel),
            Y = 2,
            Width = Dim.Fill(),
        };

        var promptWidth = passwordLabel.Bounds.Width + this._vaultPassword.Bounds.Width;
        var title = ustring.Make(str: "Vault password");
        // Create Dialog (retain backwards compat by supporting specifying height/width)
        if ((width == 0) & (height == 0))
        {
            this._passwordDialog = new Dialog(title: title,
                buttons: this._buttons);
            this._passwordDialog.Height = MesssageBoxHeight;
        }
        else
        {
            this._passwordDialog = new Dialog(title: title,
                width: Math.Max(val1: width,
                    val2: promptWidth) + 4, // +4 for padding?
                height: height,
                buttons: this._buttons);
        }

        if (border != null) this._passwordDialog.Border = border;

        // Dynamically size Width
        var messageBoxWidth = Math.Max(val1: DefaultWidth,
            val2: Math.Max(val1: title.RuneCount + 8,
                val2: Math.Max(val1: promptWidth + 4,
                    val2: buttonWidth) + 8)); // textWidth + (left + padding + padding + right)
        this._passwordDialog.Width = messageBoxWidth;

        // Setup actions
        this._passwordDialogOkButton.Clicked += this.OnPasswordDialogOkClicked;

        this._passwordDialogCancelButton.Clicked += this.OnPasswordDialogCancelClicked;
    }

    public SecureString? VaultKeySecureString { get; private set; }

    public void OnPasswordDialogOkClicked()
    {
        var insecurePassword = this._vaultPassword.Text.ToString()!;
        this._vaultPassword.Text = ustring.Make(str: string.Empty);
        var result = new SecureString();
        foreach (var c in insecurePassword) result.AppendChar(c: c);
        result.MakeReadOnly();
        this.VaultKeySecureString = result;
        Application.RequestStop();
    }

    public void OnPasswordDialogCancelClicked()
    {
        this._vaultPassword.Text = ustring.Make(str: string.Empty);
        this.VaultKeySecureString = null;
        Application.RequestStop();
    }

    public async Task<bool> Run(Toplevel top, Func<Exception, bool>? errorHandler = null)
    {
        return await Task.Run(function: () =>
        {
            // now run the password dialog
            Application.Run(view: top);

            if (this.VaultKeySecureString is null) return false;

            if (this.VaultKeySecureString is { } secureString &&
                (secureString.IsReadOnly() is false || secureString.Length == 0))
                return false;

            return true;
        });
    }
}