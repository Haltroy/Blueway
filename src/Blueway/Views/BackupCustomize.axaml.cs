using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using System.Linq;

namespace Blueway.Views
{
    public partial class BackupCustomize : AUC
    {
        public BackupCustomize()
        {
            InitializeComponent();
        }

        private AUC GoBackTo;

        public override AUC? ReturnTo(MainWindow.Buttons buttons) => GoBackTo;

        public BackupCustomize GoBackToAUC(AUC auc)
        {
            GoBackTo = auc;
            return this;
        }

        private BackupCustomize LoadActions()
        {
            MenuFlyout mfo = new();
            AddNew.Flyout = mfo;

            var ass = System.Reflection.Assembly.GetExecutingAssembly();
            //var types = ass.GetTypes();

            var type = typeof(BackupAction);
            var types = System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p)).ToArray();

            for (int i = 0; i < types.Length; i++)
            {
                // TODO: Get properties, generate object from type and generate appropriate controls that will change them and add them to schema.
            }

            return this;
        }

        private void BrowseFolder(object? s, RoutedEventArgs e)
        {
            // TODO
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