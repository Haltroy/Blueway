namespace Blueway.Views
{
    public partial class Home : AUC
    {
        public override AUC ReturnTo(MainWindow.Buttons buttons) => new Home();

        public override MainWindow.Buttons DisplayButtons => MainWindow.Buttons.None;

        public Home()
        {
            InitializeComponent();
            Title = "Home";
        }

        private void NewBackup(object? s, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (MainWindow != null)
            {
                // TODO: Backup the latest entry, otherwise create a custom one instead.
                MainWindow.SwitchTo(new BackupProcess());
            }
        }

        private void CustomBackup(object? s, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (MainWindow != null)
            {
                MainWindow.SwitchTo(new BackupCustomize());
            }
        }

        private void ApplyBackup(object? s, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (MainWindow != null)
            {
                // TODO: Show a window to select which backup to load from or add backup source.
                MainWindow.SwitchTo(new BackupProcess());
            }
        }

        private void AutoBackups(object? s, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (MainWindow != null)
            {
                MainWindow.SwitchTo(new AutoBackups());
            }
        }
    }
}