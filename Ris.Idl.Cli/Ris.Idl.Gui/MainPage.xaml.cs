using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using Ris.Idl.Gui.Pages;
using Ris.Idl.Gui.ViewModel;

namespace Ris.Idl.Gui;

/// <summary>
/// The main page that composes the generator and output log views.
/// </summary>
public sealed partial class MainPage : Page
{
    private MainViewModel? _viewModel;
    
    public MainPage()
    {
        InitializeComponent();
        
        var mainWindow = App.MainWindow!;
        
        _viewModel = DataContext as MainViewModel;
        _viewModel!.Frame = ContentFrame;
        
        mainWindow.Closed += (_, _) => _viewModel?.SaveSettings();
        mainWindow.Activated += (_, _) => _viewModel?.LoadSettings();
        
        NavigateToConvertPage();
    }

    private void NavigateToConvertPage()
    {
        ContentFrame.Navigate(typeof(ConvertPage), new ConvertPageViewModel
        {
            MainViewModel = DataContext as MainViewModel
        });
    }

    private void NavView_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
            if (args.SelectedItemContainer is not NavigationViewItem item)
                return;

            switch (item.Tag?.ToString())
            {
                case "CreateNew":
                    NavigateToConvertPage();
                    break;

                case "Settings":
                    ContentFrame.Navigate(typeof(SettingsPage));
                    break;
            }
    }
}