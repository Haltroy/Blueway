using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Blueway.Views;

public partial class MainWindow : Window
{
    private Subject<bool> BackAvailable = new Subject<bool>();
    private Subject<bool> ForwardAvailable = new Subject<bool>();
    private Subject<bool> CancelAvailable = new Subject<bool>();
    private Subject<bool> OKAvailable = new Subject<bool>();

    private List<EventHandler> okdelegates = new List<EventHandler>();
    private List<EventHandler> canceldelegates = new List<EventHandler>();

    private event EventHandler? OKPress;

    public event EventHandler OnOKPressed
    {
        add
        {
            OKPress += value;
            okdelegates.Add(value);
        }
        remove
        {
            OKPress -= value;
            okdelegates.Remove(value);
        }
    }

    private event EventHandler? CancelPress;

    public event EventHandler OnCancelPressed
    {
        add
        {
            CancelPress += value;
            canceldelegates.Add(value);
        }
        remove
        {
            CancelPress -= value;
            canceldelegates.Remove(value);
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

    public void SwitchTo(AUC uc, Buttons buttons = Buttons.None, bool clear = false)
    {
        if (ContentCarousel.Items is Avalonia.Controls.ItemCollection list)
        {
            if (clear)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] is AUC auc)
                    {
                        auc.MainWindow = null;
                        list.Remove(auc);
                    }
                }
                list.Clear();
            }
            // TODO: Get page titles from languages
            PageTitle.Text = uc.Title;
            uc.MainWindow = this;
            uc.DataContext = DataContext;
            list.Add(uc);
            ContentCarousel.SelectedIndex = ContentCarousel.ItemCount - 1;
            BackAvailable.OnNext(buttons == Buttons.Back);
            ForwardAvailable.OnNext(buttons == Buttons.Forward);
            OKAvailable.OnNext(buttons == Buttons.OK);
            CancelAvailable.OnNext(buttons == Buttons.Cancel);
        }
    }

    private void BackPressed(object? s, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (ContentCarousel.SelectedIndex > 0)
        {
            ContentCarousel.SelectedIndex--;
        }
        UpdateButtons();
    }

    private void ForwardPressed(object? s, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (ContentCarousel.SelectedIndex <= ContentCarousel.ItemCount)
        {
            ContentCarousel.SelectedIndex++;
        }
        UpdateButtons();
    }

    private void OKPressed(object? s, Avalonia.Interactivity.RoutedEventArgs e)
    {
        OKPress?.Invoke(s, e);
        for (int i = 0; i < okdelegates.Count; i++)
        {
            OnOKPressed -= okdelegates[i];
        }
        UpdateButtons();
    }

    private void CancelPressed(object? s, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CancelPress?.Invoke(s, e);
        for (int i = 0; i < canceldelegates.Count; i++)
        {
            OnCancelPressed -= canceldelegates[i];
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
}