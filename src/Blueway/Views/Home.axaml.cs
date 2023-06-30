using Avalonia.Controls;

namespace Blueway.Views
{
    public partial class Home : AUC
    {
        public override AUC? ReturnTo(MainWindow.Buttons buttons) => null;

        public override MainWindow.Buttons DisplayButtons => MainWindow.Buttons.None;

        public Home()
        {
            InitializeComponent();
            Title = "Home";
        }

        private void NewBackup(object? s, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (MainWindow != null && Settings != null)
            {
                MainWindow.SwitchTo(
                    Settings.GetLatestBackup() is BackupHistoryItem item ?
                    new BackupProcess().LoadSchema(item.Schema)
                    :
                    new BackupCustomize());
            }
        }

        private void CustomBackup(object? s, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MainWindow?.SwitchTo(new BackupCustomize());
        }

        private async void ApplyBackup(object? s, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (MainWindow != null && MainWindow.StorageProvider.CanPickFolder)
            {
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    // TODO: add translation here
                    var folders = await MainWindow.StorageProvider.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions() { AllowMultiple = false, Title = "Open a backup..." });

                    if (folders != null && folders.Count > 0)
                    {
                        MainWindow.SwitchTo(new BackupProcess().ApplyFromFolder(folders[0].Path.AbsolutePath));
                    }
                }, Avalonia.Threading.DispatcherPriority.Input);
            }
        }

        private void AutoBackups(object? s, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (MainWindow != null && Settings != null)
            {
                MainWindow.SwitchTo(new AutoBackups());
            }
        }

        private void OnHistoryItemClick(object? s, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (s is Button button && button.Tag is BackupHistoryItem item && MainWindow != null)
            {
                switch (item.Status)
                {
                    case BackupStatus.OnGoing:
                        MainWindow.SwitchTo(new BackupProcess().LoadSchema(item));
                        break;

                    case BackupStatus.Success:
                    case BackupStatus.Failure:
                    case BackupStatus.Planned:
                    default:
                        MainWindow.SwitchTo(new BackupCustomize().LoadSchema(item));
                        break;
                }
            }
        }

        private void OnHistoryItemDeleteClick(object? s, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (s is Button button && button.Tag is BackupHistoryItem item && MainWindow != null && Settings != null)
            {
                Settings.History.Remove(item);
                MainWindow.RefreshTheme();
            }
        }

        private void ClearItems(object? s, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Settings != null && MainWindow != null)
            {
                Settings.History.Clear();
                MainWindow.RefreshTheme();
            }
        }
    }
}