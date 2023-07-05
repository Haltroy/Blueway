using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using MessageBox.Avalonia.Models;
using MsBox.Avalonia;
using System;
using static System.Collections.Specialized.BitVector32;

namespace Blueway.Views
{
    public partial class Settings : AUC
    {
        public override AUC? ReturnTo(MainWindow.Buttons buttons) => null;

        public override MainWindow.Buttons DisplayButtons => MainWindow.Buttons.OK;

        public Settings()
        {
            InitializeComponent();
            Title = "Settings";
            Initialized += (s, e) => LoadSettings();
        }

        private Settings LoadSettings()
        {
            if (Settings is Blueway.Settings settings)
            {
                CheckInternet.IsChecked = settings.CheckInternetConnection;
                NotifyOnAutoBackup.IsChecked = settings.AutoBackupShowNotifications;
                CopySettings.IsChecked = settings.CopySettingsToBackup;
                NotifyOnUpdate.IsChecked = settings.ShowNotificationOnUpdate;
                AutoUpdate.IsChecked = settings.AutoUpdate;
                StartWithOS.IsChecked = settings.StartOnOS;
                StartInTray.IsChecked = settings.StartMinimized;
                ThreadCountText.Text = "" + settings.ThreadCount;
                ThreadCount.Value = settings.ThreadCount;
                CheckInternet.IsCheckedChanged += (s, e) => settings.CheckInternetConnection = CheckInternet.IsChecked ?? true;
                NotifyOnAutoBackup.IsCheckedChanged += (s, e) => settings.AutoBackupShowNotifications = NotifyOnAutoBackup.IsChecked ?? true;
                CopySettings.IsCheckedChanged += (s, e) => settings.CopySettingsToBackup = CopySettings.IsChecked ?? true;
                NotifyOnUpdate.IsCheckedChanged += (s, e) => settings.ShowNotificationOnUpdate = NotifyOnUpdate.IsChecked ?? true;
                AutoUpdate.IsCheckedChanged += (s, e) => settings.AutoUpdate = AutoUpdate.IsChecked ?? true;
                StartWithOS.IsCheckedChanged += (s, e) => settings.StartOnOS = StartWithOS.IsChecked ?? true;
                StartInTray.IsCheckedChanged += (s, e) => settings.StartMinimized = StartInTray.IsChecked ?? true;
                ThreadCount.PropertyChanged += (s, e) =>
                {
                    if (e.Property == Slider.ValueProperty)
                    {
                        settings.ThreadCount = (int)ThreadCount.Value;
                        ThreadCountText.Text = "" + settings.ThreadCount;
                    }
                };

                for (int i = 0; i < settings.Themes.Count; i++)
                {
                    Themes.Items.Add(new ComboBoxItem() { Content = settings.Themes[i].Name, Tag = settings.Themes[i] });
                    if (settings.Themes[i] == settings.CurrentTheme)
                    {
                        Themes.SelectedIndex = i;
                    }
                }

                Themes.SelectionChanged += (s, e) =>
                {
                    settings.CurrentTheme = Themes.Items[Themes.SelectedIndex] is ComboBoxItem cbi && cbi.Tag is Theme theme ? theme : settings.CurrentTheme;
                    MainWindow?.RefreshTheme();
                };

                LoadAllSources(settings);
            }
            return this;
        }

        public Settings LoadAllSources(Blueway.Settings settings)
        {
            for (int i = 0; i < settings.Sources.Count; i++)
            {
                SourcesPanel.Children.Add(GenerateSource(settings.Sources[i]));
            }
            return this;
        }

        public StackPanel GenerateSource(Blueway.AppSource source)
        {
            StackPanel panel = new() { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 5 };
            Button delButton = new();
            delButton.Click += async (s, e) =>
            {
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    var msgbox = MessageBoxManager.GetMessageBoxCustom(new MsBox.Avalonia.Dto.MessageBoxCustomParams()
                    {
                        CanResize = false,
                        ShowInCenter = true,
                        ContentTitle = "Blueway", //TODO: Add translations
                        ContentHeader = "Do you really want to remove this source?",
                        ContentMessage = source.Name,
                        Icon = MsBox.Avalonia.Enums.Icon.Question,
                        WindowIcon = MainWindow?.Icon,
                        ButtonDefinitions = new ButtonDefinition[]
                        {
                        new ButtonDefinition() { IsDefault = true, Name = "Yes" },
                        new ButtonDefinition() { IsCancel = true, Name = "No" },
                        }
                    });

                    string result = await msgbox.ShowWindowDialogAsync(MainWindow);
                    if (string.Equals(result, "Yes"))
                    {
                        source.Delete();
                        SourcesPanel.Children.Remove(panel);
                    }
                });
            };
            Panel delButtonpanel = new();
            Image delButton_b = new();
            delButton_b.Bind(IsVisibleProperty, IsNotDark_ShadowButton.GetObservable(IsEnabledProperty));
            Image delButton_w = new();
            delButton_w.Bind(IsVisibleProperty, IsDark_ShadowButton.GetObservable(IsEnabledProperty));
            delButtonpanel.Children.Add(delButton_b);
            delButtonpanel.Children.Add(delButton_w);

            delButton.Content = delButtonpanel;
            panel.Children.Add(delButton);

            StackPanel detailsPanel = new() { Orientation = Avalonia.Layout.Orientation.Vertical, Spacing = 5 };

            StackPanel titlePanel = new() { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 5 };
            detailsPanel.Children.Add(titlePanel);

            Image sourceLogo = new()
            {
                Width = 16,
                Height = 16,
                Source = System.IO.File.Exists(source.IconPath)
                ? new Bitmap(new System.IO.FileStream(source.IconPath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                : (Avalonia.Media.IImage)new Bitmap(AssetLoader.Open(new Uri("/Assets/blueway-logo.png")))
            };

            titlePanel.Children.Add(sourceLogo);

            TextBlock title = new() { FontSize = 20, Text = source.Name };
            titlePanel.Children.Add(title);

            TextBlock desc = new() { Text = source.Description };
            detailsPanel.Children.Add(desc);

            return panel;
        }

        public void AddNew(object? s, RoutedEventArgs e)
        {
            // TODO
        }
    }
}