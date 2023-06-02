// Ignore Spelling: App

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Blueway.ViewModels;
using Blueway.Views;
using System;
using System.Linq;

namespace Blueway;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            }.ShowTrayIcon().AsBackground(desktop.Args.Contains("--background") || desktop.Args.Contains("--bg"));
            desktop.Exit += (s, e) => { if (desktop.MainWindow is MainWindow mw && mw.DataContext is ViewModelBase vmb) { vmb.Settings.Save(); } };
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static Action<int> Shutdown;
}