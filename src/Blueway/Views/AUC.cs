using Avalonia.Controls;

namespace Blueway.Views
{
    /// <summary>
    /// Advanced <see cref="UserControl"/>. For Blueway.
    /// </summary>
    public abstract class AUC : UserControl
    {
        /// <summary>
        /// The Main Window.
        /// </summary>
        public MainWindow? MainWindow { get; set; }

        /// <summary>
        /// Title text to look at dictionary to find the real translated title name.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        public ViewModels.ViewModelBase? ViewModel => MainWindow is null ? null : (MainWindow.DataContext is ViewModels.ViewModelBase vmb ? vmb : null);

        /// <summary>
        /// Gets the settings.
        /// </summary>
        public Blueway.Settings? Settings => ViewModel is ViewModels.ViewModelBase vmb ? vmb.Settings : null;

        /// <summary>
        /// Determines which buttons to show on the main window.
        /// </summary>
        public abstract MainWindow.Buttons DisplayButtons { get; }

        /// <summary>
        /// Determines which screen to return when a button is pressed.
        /// <para />
        /// Return <see cref="null"/> for <seealso cref="Home"/>.
        /// </summary>
        /// <param name="button">The pressed button.</param>
        /// <returns><see cref="AUC"/> to return to or null for the home screen.</returns>
        public abstract AUC? ReturnTo(MainWindow.Buttons button);
    }
}