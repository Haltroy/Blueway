using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using System.Threading.Tasks;

namespace Blueway.Views
{
    public partial class BackupCustomize : AUC
    {
        public BackupCustomize()
        {
            InitializeComponent();
            Initialized += (s, e) => LoadActions();
        }

        private AUC? GoBackTo;
        private BackupSchema Schema { get; set; } = new(new());

        public override AUC? ReturnTo(MainWindow.Buttons buttons) => GoBackTo;

        internal BackupCustomize LoadSchema(BackupSchema backupSchema)
        {
            for (int i = 0; i < backupSchema.Actions.Count; i++)
            {
                ActionList.Children.Add(GenerateFromAction(backupSchema.Actions[i], backupSchema.Actions[i].GetBackupActionType));
            }
            return this;
        }

        internal BackupCustomize LoadSchema(BackupHistoryItem item)
        {
            return LoadSchema(item);
        }

        public BackupCustomize GoBackToAUC(AUC auc)
        {
            GoBackTo = auc;
            return this;
        }

        private Button GenerateFromAction(BackupAction action, BackupActionType type)
        {
            // TODO
            throw new System.NotImplementedException();
        }

        private BackupCustomize LoadActions()
        {
            if (Settings != null)
            {
                MenuFlyout mfo = new();
                AddNew.Flyout = mfo;

                for (int i = 0; i < Settings.BackupActionTypes.Length; i++)
                {
                    MenuItem item = new() { Header = Settings.BackupActionTypes[i].Name };
                    mfo.Items.Add(item);
                    item.Click += (s, e) =>
                    {
                        var action = Settings.BackupActionTypes[i].GenerateAction();
                        Schema.Actions.Add(action);
                    };
                }
            }
            return this;
        }

        private async void BrowseFolder(object? s, RoutedEventArgs e)
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
            {
                if (MainWindow != null && MainWindow.StorageProvider.CanPickFolder)
                {
                    // TODO: Add translations
                    var files = await MainWindow.StorageProvider.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions() { AllowMultiple = false, Title = "Open a folder..." });
                    if (files != null && files.Count > 0)
                    {
                        BackupTo.Text = files[0].Path.AbsolutePath;
                    }
                }
            }, Avalonia.Threading.DispatcherPriority.Input);
        }

        private void OpenFlyout(object? s, RoutedEventArgs e)
        {
            if (s == null) { return; }
            if (s is Button btn && btn.Flyout is FlyoutBase flyout)
            {
                flyout.ShowAt(btn);
            }
        }

        public override MainWindow.Buttons DisplayButtons => MainWindow.Buttons.Back;
    }
}