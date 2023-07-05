using Avalonia.Controls;

namespace Blueway.Views
{
    public partial class Dialog : AUC
    {
        private MainWindow.Buttons Buttons { get; set; }
        private AUC? Caller { get; set; }

        public Dialog()
        {
            InitializeComponent();
        }

        public override MainWindow.Buttons DisplayButtons => Buttons;

        public override AUC? ReturnTo(MainWindow.Buttons button) => Caller;
    }
}