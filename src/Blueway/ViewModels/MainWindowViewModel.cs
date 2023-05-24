using Avalonia.Media;
using System.Reflection;
using Blueway;
using Avalonia.Themes.Fluent;
using Avalonia;
using Avalonia.Platform;

namespace Blueway.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public static string AppName =>
        System.Reflection.Assembly.GetExecutingAssembly() is Assembly ass
        && ass.GetName() is AssemblyName name
        && !string.IsNullOrWhiteSpace(name.Name)
            ? name.Name
            : "Blueway";

    public static string AppVersion =>
        "v"
        + (
            System.Reflection.Assembly.GetExecutingAssembly() is Assembly ass
            && ass.GetName() is AssemblyName name
            && name.Version != null
                ? Tools.FormatVersion(name.Version)
                : "?"
        );

    public IBrush BackColor =>
        new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse(Settings.CurrentTheme.Background.ToHex(false)));

    public IBrush BackColor2 =>
        new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse(Settings.CurrentTheme.Background.ShiftBrightness(20).ToHex(false)));

    public IBrush OverlayColor =>
    new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse(Settings.CurrentTheme.Accent.ToHex(false)));

    public IBrush OverlayColor2 =>
    new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse(Settings.CurrentTheme.Accent.ShiftBrightness(20).ToHex(false)));

    public IBrush OverlayColor3 =>
    new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse(Settings.CurrentTheme.Accent.ShiftBrightness(40).ToHex(false)));

    public IBrush ForeColor =>
        new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse(Settings.CurrentTheme.Foreground.ToHex(false)));

    public IBrush ForeColor2 =>
        new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse(Settings.CurrentTheme.Foreground.ShiftBrightness(20).ToHex(false)));

    public IBrush ForeColor3 =>
        new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse(Settings.CurrentTheme.Foreground.ShiftBrightness(40).ToHex(false)));

    public bool IsDark => !Settings.CurrentTheme.Background.IsBright;

    public Avalonia.Controls.WindowTransparencyLevel TransparencyLevel => Settings.CurrentTheme.UseAcrylic ?
    Avalonia.Controls.WindowTransparencyLevel.AcrylicBlur : Avalonia.Controls.WindowTransparencyLevel.None;

    public string OK => "OK";
    public string Cancel => "Cancel";
    public string Back => "Back";
    public string Forward => "Forward";
}