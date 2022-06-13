using PhantomKit.Models.Menus;
using PhantomMail.CLI.Commands;

namespace PhantomMail.CLI.Menus;

public class PhantomMailMainMenu : PhantomKitMainMenu
{
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    public PhantomMailMainMenu(PhantomMailGuiCommand guiCommand) : base(guiCommand: guiCommand)
    {
    }

    protected new PhantomMailGuiCommand GuiCommand => (PhantomMailGuiCommand)base.GuiCommand;
}