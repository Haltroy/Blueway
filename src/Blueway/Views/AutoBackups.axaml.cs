using Avalonia.Controls;

namespace Blueway.Views
{
    public partial class AutoBackups : AUC
    {
        public AutoBackups()
        {
            InitializeComponent();
        }

        public AutoBackups GenAutoBackups(Settings settings)
        {
            // TODO
            throw new System.NotImplementedException();
        }

        private StackPanel GenPanel(BackupHistoryItem item)
        {
            // TODO
            throw new System.NotImplementedException();
        }

        public override MainWindow.Buttons DisplayButtons => MainWindow.Buttons.Back;

        public override AUC ReturnTo(MainWindow.Buttons button) => new Home();
    }
}