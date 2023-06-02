using Avalonia.Controls;

namespace Blueway.Views
{
    public partial class About : AUC
    {
        public About()
        {
            InitializeComponent();
            Title = "About";
            Initialized += (s, e) => { License.Text = Tools.ReadResource("Blueway.LICENSE"); };
        }

        public override AUC ReturnTo(MainWindow.Buttons buttons) => new Home();

        public override MainWindow.Buttons DisplayButtons => MainWindow.Buttons.OK;

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