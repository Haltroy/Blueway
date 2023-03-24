using Avalonia.Media;
using System.Reflection;
using Blueway;

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

    public IBrush BackColor { get; set; } =
        new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#80ffffff"));

    public IBrush ForeColor { get; set; } =
        new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#000000"));

    public IBrush ForeColor2 { get; set; } =
        new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#202020"));

    public bool IsDark { get; set; } = true;

    public Avalonia.Controls.WindowTransparencyLevel TransparencyLevel { get; set; } =
    Avalonia.Controls.WindowTransparencyLevel.AcrylicBlur;

    public string OK => "OK";
    public string Cancel => "Cancel";
    public string Back => "Back";
    public string Forward => "Forward";
}