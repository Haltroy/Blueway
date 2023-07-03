// Ignore Spelling: auc

using Avalonia.Controls;

namespace Blueway.Views
{
    public partial class BackupProcess : AUC
    {
        private AUC? GoBackTo;

        public override AUC? ReturnTo(MainWindow.Buttons buttons) => GoBackTo;

        public BackupProcess GoBackToAUC(AUC auc)
        {
            GoBackTo = auc;
            return this;
        }

        public override MainWindow.Buttons DisplayButtons => MainWindow.Buttons.Cancel;

        public BackupProcess()
        {
            InitializeComponent();
        }

        internal BackupProcess ApplyFromFolder(string filename)
        {
            // TODO
            return this;
        }

        internal BackupProcess LoadSchema(BackupHistoryItem item) => LoadSchema(item.Schema);

        internal BackupProcess LoadSchema(BackupSchema schema)
        {
            schema.OnProgressChange += (s, p) =>
            {
                TotalProgress.Maximum = p.Total;
                TotalProgress.Value = p.Current;
                TotalProgress.IsIndeterminate = p.IsIndeterminate;
                TotalPerc.Text = p.IsIndeterminate ? "Unknown" /* TODO: Add translation here */ : (p.Percentage + "%");

                // NOTE: Check the note below.
                // TotalLeft.Text =  Get here the value, also translate.
            };
            for (int i = 0; i < schema.Actions.Count; i++)
            {
                var action = schema.Actions[i];
                GenerateAction(action);
            }
            return this;
        }

        private StackPanel GenerateAction(BackupAction action)
        {
            StackPanel actionPanel = new() { Orientation = Avalonia.Layout.Orientation.Horizontal };

            Panel imagePanel = new();

            Image waitingImage = new() { Width = 16, Height = 16 };
            Image doneImage = new() { Width = 16, Height = 16, IsVisible = false };
            AvaloniaProgressRing.ProgressRing ring = new() { Width = 80, Height = 20, IsActive = true, IsVisible = false };

            imagePanel.Children.Add(waitingImage);
            imagePanel.Children.Add(doneImage);
            imagePanel.Children.Add(ring);

            actionPanel.Children.Add(imagePanel);

            // TODO: Add language support here.
            TextBlock actionName = new() { Text = action.Name };
            actionPanel.Children.Add(actionName);

            action.OnDone += (s) => { doneImage.IsVisible = true; waitingImage.IsVisible = false; ring.IsVisible = false; };
            action.OnStart += (s) =>
            {
                doneImage.IsVisible = false;
                waitingImage.IsVisible = false;
                ring.IsVisible = true;
                CurrentName.Text = actionName.Text;
            };
            action.OnProgressChange += (s, p) =>
            {
                CurrentProgress.Maximum = p.Total;
                CurrentProgress.Value = p.Current;
                CurrentProgress.IsIndeterminate = p.IsIndeterminate;
                CurrentPerc.Text = p.IsIndeterminate ? "Unknown" /* TODO: Add translation here */ : (p.Percentage + "%");

                // NOTE: I have no idea how should i do the "some time left" thing accurately until someone makes 3h video essay on how Windows is bad at telling users and shows weird method for getting more accurate time left. I also have to store the percentages and their marks to calculate it and making that sounds too time consuming so im not gonna do that for this RC
                // CurrentLeft.Text =  Get here the value, also translate.
            };

            return actionPanel;
        }
    }
}