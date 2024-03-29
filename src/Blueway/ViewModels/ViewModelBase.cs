using ReactiveUI;

namespace Blueway.ViewModels;

public class ViewModelBase : ReactiveObject
{
    public Settings Settings { get; set; } = new Settings().AutoLoadConfig().AutoLoadExtensions();
}