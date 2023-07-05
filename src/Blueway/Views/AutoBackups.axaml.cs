using Avalonia.Controls;

namespace Blueway.Views
{
    public partial class AutoBackups : AUC
    {
        public AutoBackups()
        {
            InitializeComponent();
        }

        private void OnItemClick(object? s, Avalonia.Interactivity.RoutedEventArgs e)
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

        private void OnItemDeleteClick(object? s, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (s is Button button && button.Tag is BackupHistoryItem item && MainWindow != null && Settings != null)
            {
                Settings.History.Remove(item);
                MainWindow.RefreshTheme();
            }
        }

        public override MainWindow.Buttons DisplayButtons => MainWindow.Buttons.Back;

        public override AUC? ReturnTo(MainWindow.Buttons button) => null;
    }
}