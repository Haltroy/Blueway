using Avalonia.Controls;

namespace Blueway.Views
{
    public partial class Home : AUC
    {
        public Home()
        {
            InitializeComponent();
            Title = "Home";
        }

        private void NewBackup(object? s, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (MainWindow != null)
            {
                // TODO: Backup the latest entry
                MainWindow.SwitchTo(new BackupProcess(), MainWindow.Buttons.Cancel, true);
            }
        }

        private void CustomBackup(object? s, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (MainWindow != null)
            {
                MainWindow.SwitchTo(new BackupCustomize(), MainWindow.Buttons.Back, false);
            }
        }

        private void ApplyBackup(object? s, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (MainWindow != null)
            {
                // TODO: Show a window to select which backup to load from or add backup source.
                MainWindow.SwitchTo(new BackupDetails(), MainWindow.Buttons.Cancel, true);
            }
        }

        private void OpenSettings(object? s, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (MainWindow != null)
            {
                MainWindow.OnOKPressed += (s, e) =>
                {
                    if (s is Button button && button.Parent is StackPanel panel && panel.Parent is Grid grid && grid.Parent is MainWindow mw)
                    {
                        mw.SwitchTo(this, MainWindow.Buttons.None, true);
                    }
                };
                MainWindow.SwitchTo(new Settings(), MainWindow.Buttons.OK, true);
            }
        }

        private void AutoBackups(object? s, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (MainWindow != null)
            {
                // TODO: Show a window to select which auto-backup to edit or to create a new auto-backup
            }
        }
    }
}