using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Blueway.Views
{
    public partial class Settings : AUC
    {
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
                // General - Settings
                CheckInternet.IsChecked = settings.CheckInternetConnection;
                NotifyOnAutoBackup.IsChecked = settings.AutoBackupShowNotifications;
                CopySettings.IsChecked = settings.CopySettingsToBackup;
                NotifyOnUpdate.IsChecked = settings.ShowNotificationOnUpdate;
                AutoUpdate.IsChecked = settings.AutoUpdate;
                StartWithOS.IsChecked = settings.StartOnOS;
                StartInTray.IsChecked = settings.StartMinimized;
                ThreadCountText.Text = "" + settings.ThreadCount;
                ThreadCount.Value = settings.ThreadCount;

                // General - Events
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

                // TODO: Sources here

                /*

<StackPanel Orientation="Horizontal" Spacing="5">
						<Button Content="Delete Image here" />
						<StackPanel Orientation="Vertical">
							<StackPanel Orientation="Horizontal" Spacing="5">
								<TextBlock Text="Source Name Here" FontSize="20" />
								<Image Source="Source Icon here" Width="16" Height="16" />
							</StackPanel>
							<TextBlock Text="Source Description Here" />
						</StackPanel>
					</StackPanel>

                */

                // Customization - Settings
                if (Themes.Items is System.Collections.IList list)
                {
                    for (int i = 0; i < settings.Themes.Count; i++)
                    {
                        list.Add(new ComboBoxItem() { Content = settings.Themes[i].Name, Tag = settings.Themes[i] });
                    }
                }
                Themes.SelectedIndex = settings.Themes.IndexOf(settings.CurrentTheme) is int index && index != -1 ? index : 0;
                // TODO: when these change, clone the theme.
                BackColor.Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse(settings.CurrentTheme.Background.ToHex()));
                ForeColor.Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse(settings.CurrentTheme.Foreground.ToHex()));
                UseBlur.IsChecked = settings.CurrentTheme.UseAcrylic;

                // Customization - Events
                Themes.SelectionChanged += (s, e) =>
                {
                    settings.CurrentTheme = Themes.Items is System.Collections.IList list && list[Themes.SelectedIndex] is ComboBoxItem cbi && cbi.Tag is Theme theme ? theme : settings.CurrentTheme;
                    if (MainWindow != null)
                    { MainWindow.RefreshTheme(); }
                    var dataContext = ThemePreview.DataContext;
                    ThemePreview.DataContext = null;
                    ThemePreview.InvalidateVisual();
                    ThemePreview.DataContext = dataContext;
                    ThemePreview.InvalidateVisual();
                };
            }
            return this;
        }

        private void Navigate(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Control control && control.Tag is string link)
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    UseShellExecute = true,
                    FileName = link
                });
            }
        }
    }
}