using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Blueway.Views;

public partial class MainWindow : Window
{
    private readonly Subject<bool> BackAvailable = new();
    private readonly Subject<bool> ForwardAvailable = new();
    private readonly Subject<bool> CancelAvailable = new();
    private readonly Subject<bool> OKAvailable = new();

    public Home HomeScreen;

    private bool allowClose = false;

    public MainWindow ShowTrayIcon()
    {
        TrayIcon tray = new()
        {
            Icon = Icon
        };
        tray.Clicked += (s, e) => { if (IsVisible) { Hide(); } else { Show(); } };

        NativeMenu menu = new();

        // TODO: Add translations

        NativeMenuItem showMW = new() { Header = "Show" };
        showMW.Click += (s, e) => Show();
        menu.Items.Add(showMW);

        menu.Items.Add(new NativeMenuItemSeparator());

        NativeMenuItem showHome = new() { Header = "Home" };
        showHome.Click += (s, e) => { Show(); HomeScreen = new Home(); SwitchTo(null); };
        menu.Items.Add(showHome);

        NativeMenuItem showAuto = new() { Header = "Automated backups..." };
        showAuto.Click += (s, e) => { Show(); SwitchTo(new AutoBackups()); };
        menu.Items.Add(showAuto);

        NativeMenuItem showSettings = new() { Header = "Settings" };
        showSettings.Click += (s, e) => { Show(); OpenSettings(s, new Avalonia.Interactivity.RoutedEventArgs()); };
        menu.Items.Add(showSettings);

        NativeMenuItem showAbout = new() { Header = "About" };
        showAbout.Click += (s, e) => { Show(); OpenAbout(s, new Avalonia.Interactivity.RoutedEventArgs()); };
        menu.Items.Add(showAbout);

        menu.Items.Add(new NativeMenuItemSeparator());

        NativeMenuItem exit = new() { Header = "Exit" };
        exit.Click += (s, e) =>
        {
            tray.IsVisible = false;
            tray.Dispose();
            allowClose = true;
            Close();
            if (Avalonia.Application.Current != null && Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                lifetime.TryShutdown(0);
            }
        };
        menu.Items.Add(exit);

        tray.IsVisible = true;
        tray.Menu = menu;

        return this;
    }

    private bool IsBackground = false;

    public MainWindow AsBackground(bool bg = false)
    {
        IsBackground = bg;
        return this;
    }

    internal void RefreshTheme(Theme? theme = null)
    {
        if (DataContext is ViewModels.ViewModelBase vmb)
        {
            if (theme is not null) { vmb.Settings.CurrentTheme = theme; }
            DataContext = null;
            DataContext = vmb;
            InvalidateVisual();
            for (int i = 0; i < ContentCarousel.Items.Count; i++)
            {
                if (ContentCarousel.Items[i] is Control control)
                {
                    control.DataContext = null;
                    control.DataContext = vmb;
                    control.InvalidateVisual();
                }
            }
        }
    }

    public MainWindow()
    {
        InitializeComponent();
        Back.Bind(IsVisibleProperty, BackAvailable);
        Back.Bind(IsEnabledProperty, BackAvailable);
        Forward.Bind(IsVisibleProperty, ForwardAvailable);
        Forward.Bind(IsEnabledProperty, ForwardAvailable);
        OK.Bind(IsVisibleProperty, OKAvailable);
        OK.Bind(IsEnabledProperty, OKAvailable);
        Cancel.Bind(IsVisibleProperty, CancelAvailable);
        Cancel.Bind(IsEnabledProperty, CancelAvailable);
        HomeScreen = new Home();
        SwitchTo(HomeScreen);
        RefreshTheme();
        Activated += MW_Activated;
    }

    private void MW_Activated(object? sender, EventArgs e)
    {
        if (IsBackground)
        {
            Hide();
            Activated -= MW_Activated;
        }
    }

    private void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (!allowClose)
        {
            e.Cancel = true;
            Hide();
        }
    }

    [Flags]
    public enum Buttons
    {
        None,
        Back,
        Forward,
        OK,
        Cancel
    }

    public void SwitchTo(AUC? uc = null)
    {
        HomeScreen ??= new Home();
        uc ??= HomeScreen;

        // TODO: Get page titles from languages
        PageTitle.Text = uc.Title;
        uc.MainWindow = this;
        uc.DataContext = DataContext;

        ContentCarousel.SelectedIndex = ContentCarousel.ItemCount - 1;
        BackAvailable.OnNext(uc.DisplayButtons == Buttons.Back);
        ForwardAvailable.OnNext(uc.DisplayButtons == Buttons.Forward);
        OKAvailable.OnNext(uc.DisplayButtons == Buttons.OK);
        CancelAvailable.OnNext(uc.DisplayButtons == Buttons.Cancel);
        ContentCarousel.Items.Add(uc);

        for (int i = 0; i < ContentCarousel.Items.Count; i++)
        {
            if (ContentCarousel.Items[i] is AUC auc)
            {
                if (auc == uc || auc == ContentCarousel.SelectedItem || auc == HomeScreen) { continue; }
                auc.MainWindow = null;
                ContentCarousel.Items.Remove(auc);
            }
        }

        ContentCarousel.Next();
        CleanupCarousel();
        CleanupCarousel(20000);
    }

    private async void CleanupCarousel(int ms = 2000)
    {
        await Task.Run(() =>
        {
            System.Threading.Thread.Sleep(ms);
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                for (int i = 0; i < ContentCarousel.ItemCount; i++)
                {
                    if (i != ContentCarousel.SelectedIndex && ContentCarousel.Items[i] != HomeScreen)
                    {
                        ContentCarousel.Items.RemoveAt(i);
                    }
                }
            }, Avalonia.Threading.DispatcherPriority.Background);
            GC.Collect();
        });
    }

    private void ShowAbout(object? sender, Avalonia.Input.PointerPressedEventArgs e) => OpenAbout(sender, e);

    private void OpenAbout(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        SwitchTo(new About());
    }

    private void BackPressed(object? s, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (ContentCarousel.SelectedItem is AUC auc)
        {
            SwitchTo(auc.ReturnTo(Buttons.Back));
        }
        UpdateButtons();
    }

    private void ForwardPressed(object? s, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (ContentCarousel.SelectedItem is AUC auc)
        {
            SwitchTo(auc.ReturnTo(Buttons.Forward));
        }
        UpdateButtons();
    }

    private void OKPressed(object? s, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (ContentCarousel.SelectedItem is AUC auc)
        {
            SwitchTo(auc.ReturnTo(Buttons.OK));
        }
        UpdateButtons();
    }

    private void CancelPressed(object? s, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (ContentCarousel.SelectedItem is AUC auc)
        {
            SwitchTo(auc.ReturnTo(Buttons.Cancel));
        }
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        BackAvailable.OnNext(
            (OK.IsVisible || Cancel.IsVisible) && ContentCarousel.SelectedIndex > 0
        );
        ForwardAvailable.OnNext(
            (OK.IsVisible || Cancel.IsVisible)
&& ContentCarousel.SelectedIndex <= ContentCarousel.ItemCount
        );
    }

    private void OpenSettings(object? s, Avalonia.Interactivity.RoutedEventArgs e)
    {
        SwitchTo(new Settings());
    }
}