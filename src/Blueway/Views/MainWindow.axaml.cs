using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Blueway.Views;

public partial class MainWindow : Window
{
    private Subject<bool> BackAvailable = new Subject<bool>();
    private Subject<bool> ForwardAvailable = new Subject<bool>();
    private Subject<bool> CancelAvailable = new Subject<bool>();
    private Subject<bool> OKAvailable = new Subject<bool>();

    private bool allowClose = false;

    public MainWindow ShowTrayIcon()
    {
        TrayIcon tray = new();
        tray.Icon = this.Icon;

        NativeMenu menu = new();

        // TODO: Add more and add translations

        NativeMenuItem showMW = new() { Header = "Show" };
        showMW.Click += (s, e) => Show();
        menu.Items.Add(showMW);

        NativeMenuItem exit = new() { Header = "Exit" };
        exit.Click += (s, e) =>
        {
            tray.IsVisible = false;
            tray.Dispose();
            allowClose = true;
            Close();
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
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
        SwitchTo(new Home());
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
        if (ContentCarousel.Items is Avalonia.Controls.ItemCollection list)
        {
            if (uc is null)
            {
                uc = new Home();
            }

            // TODO: Get page titles from languages
            PageTitle.Text = uc.Title;
            uc.MainWindow = this;
            uc.DataContext = DataContext;

            ContentCarousel.SelectedIndex = ContentCarousel.ItemCount - 1;
            BackAvailable.OnNext(uc.DisplayButtons == Buttons.Back);
            ForwardAvailable.OnNext(uc.DisplayButtons == Buttons.Forward);
            OKAvailable.OnNext(uc.DisplayButtons == Buttons.OK);
            CancelAvailable.OnNext(uc.DisplayButtons == Buttons.Cancel);
            list.Add(uc);

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] is AUC auc)
                {
                    if (auc == uc || auc == ContentCarousel.SelectedItem) { continue; }
                    auc.MainWindow = null;
                    list.Remove(auc);
                }
            }

            ContentCarousel.Next();
            CleanupCarousel();
            CleanupCarousel(20000);
        }
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
                    if (i != ContentCarousel.SelectedIndex)
                    {
                        ContentCarousel.Items.RemoveAt(i);
                    }
                }
            }, Avalonia.Threading.DispatcherPriority.Background);
            GC.Collect();
        });
    }

    private void ShowAbout(object? sender, Avalonia.Input.PointerPressedEventArgs e)
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
            (OK.IsVisible || Cancel.IsVisible) ? ContentCarousel.SelectedIndex > 0 : false
        );
        ForwardAvailable.OnNext(
            (OK.IsVisible || Cancel.IsVisible)
                ? ContentCarousel.SelectedIndex <= ContentCarousel.ItemCount
                : false
        );
    }

    private void OpenSettings(object? s, Avalonia.Interactivity.RoutedEventArgs e)
    {
        SwitchTo(new Settings());
    }
}