using System.Linq;
using Avalonia.Controls;

namespace Blueway.Views
{
    public partial class BackupCustomize : AUC
    {
        public BackupCustomize()
        {
            InitializeComponent();
        }

        private BackupCustomize LoadActions()
        {
            var ass = System.Reflection.Assembly.GetExecutingAssembly();
            //var types = ass.GetTypes();

            var type = typeof(BackupAction);
            var types = System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p)).ToArray();

            for (int i = 0; i < types.Length; i++)
            {
                // TODO: Get properties, generate object from type and generate appropiate controls that will change them and add them to schema.
            }

            return this;
        }
    }
}