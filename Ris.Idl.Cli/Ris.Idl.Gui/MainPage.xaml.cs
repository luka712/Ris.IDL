using Ris.Idl.Gui.ViewModel;

namespace Ris.Idl.Gui;

/// <summary>
/// The main page that composes the generator and output log views.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();

        var mainWindow = App.MainWindow!;
        mainWindow.Closed += (_, _) => (DataContext as MainViewModel)?.SaveSettings();
        mainWindow.Activated += (_, _) => (DataContext as MainViewModel)?.LoadSettings();
    }
}