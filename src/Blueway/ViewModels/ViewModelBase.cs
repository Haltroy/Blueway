using ReactiveUI;

namespace Blueway.ViewModels;

public class ViewModelBase : ReactiveObject
{
    public Settings Settings => Main.Settings;
}