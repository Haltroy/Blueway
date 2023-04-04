using Avalonia.Controls;

namespace Blueway.Views
{
    public partial class BackupProcess : AUC
    {
        public BackupProcess()
        {
            InitializeComponent();
        }

        private BackupProcess LoadSchema(BackupSchema schema)
        {
            // TODO:
            for (int i = 0; i < schema.Actions.Count; i++)
            {
                var action = schema.Actions[i];
                //
            }
            return this;
        }
    }
}
