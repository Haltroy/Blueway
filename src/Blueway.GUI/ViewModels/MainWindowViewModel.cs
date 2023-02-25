using Avalonia.Media;
using System.Reflection;

namespace ProjectKolme.GUI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public static string AppName => System.Reflection.Assembly.GetExecutingAssembly() is Assembly ass && ass.GetName() is AssemblyName name && !string.IsNullOrWhiteSpace(name.Name)
                ? name.Name
                : "stupid ass project";

    public static string AppVersion => System.Reflection.Assembly.GetExecutingAssembly() is Assembly ass && ass.GetName() is AssemblyName name && name.Version != null
                ? name.Version.ToString()
                : "<No Version>";

    public IBrush BackColor => new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#55000000"));
    public IBrush ForeColor => new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#ffffff"));
    public IBrush ForeColor2 => new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#eeeeee"));
    public bool IsDark => true;

    public string OK => "OK";
    public string Cancel => "Cancel";
    public string Back => "Back";
    public string Forward => "Forward";
}