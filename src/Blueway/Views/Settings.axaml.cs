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
                    if (MainWindow != null)
                    { MainWindow.RefreshTheme(); }
                };
            }
            return this;
        }
    }
}