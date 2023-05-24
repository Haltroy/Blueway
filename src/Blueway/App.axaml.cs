// Ignore Spelling: App

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using Blueway.ViewModels;
using Blueway.Views;

namespace Blueway;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Styles[0].ToString();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
            desktop.Exit += (s, e) => { if (desktop.MainWindow is MainWindow mw && mw.DataContext is ViewModelBase vmb) { vmb.Settings.Save(); } };
        }

        base.OnFrameworkInitializationCompleted();
    }
}