using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blueway.Views
{
    public class AUC : UserControl
    {
        public MainWindow? MainWindow { get; set; }
        public string? Title { get; set; }
        public ViewModels.ViewModelBase? ViewModel => MainWindow is null ? null : (MainWindow.DataContext is ViewModels.ViewModelBase vmb ? vmb : null);
        public Blueway.Settings? Settings => ViewModel is ViewModels.ViewModelBase vmb ? vmb.Settings : null;
    }
}