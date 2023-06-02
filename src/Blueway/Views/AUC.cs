using Avalonia.Controls;

namespace Blueway.Views
{
    public abstract class AUC : UserControl
    {
        public MainWindow? MainWindow { get; set; }
        public string? Title { get; set; }
        public ViewModels.ViewModelBase? ViewModel => MainWindow is null ? null : (MainWindow.DataContext is ViewModels.ViewModelBase vmb ? vmb : null);
        public Blueway.Settings? Settings => ViewModel is ViewModels.ViewModelBase vmb ? vmb.Settings : null;
        public abstract MainWindow.Buttons DisplayButtons { get; }

        public abstract AUC ReturnTo(MainWindow.Buttons button);
    }
}