using McMaster.Extensions.CommandLineUtils;
using PhantomKit.Models.Themes;
using Terminal.Gui;

namespace PhantomKit.Interfaces;

public interface IGuiCommand
{
    bool DarkMode { get; set; }
    Toplevel RebuildWindow();
    int OnExecute(CommandLineApplication app);
    void OnException(Exception ex);
    void OutputToConsole(string data);
    void OutputError(string message);
    void Dispose();
    void UpdateTheme(HumanEditableTheme theme);
    void AddScrollViewChild();
    void Copy();
    void Cut();
    void Paste();
}