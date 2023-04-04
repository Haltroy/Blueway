using Avalonia.Controls;

namespace Blueway.Views
{
    public partial class Settings : AUC
    {
        public Settings()
        {
            InitializeComponent();
            Title = "Settings";
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