using Windows.Storage.Pickers;
using Ris.Idl.Gui.ViewModel;

namespace Ris.Idl.Gui.Views;

/// <summary>
/// A UserControl that provides the main generator interface for selecting
/// project type, input project, output folder, and triggering generation.
/// </summary>
public sealed partial class GeneratorView : UserControl
{
    public GeneratorView()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Gets the ViewModel from the DataContext.
    /// </summary>
    private MainViewModel? ViewModel => DataContext as MainViewModel;

    private async void BrowseProjectButton_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            ViewMode = PickerViewMode.List
        };
        picker.FileTypeFilter.Add(".csproj");

        // Get the window handle for the picker
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();
        if (file != null && ViewModel != null)
        {
            ViewModel.ProjectPath = file.Path;
            ViewModel.AppendLog($"Selected project: {file.Path}");
        }
    }

    private async void BrowseFolderButton_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FolderPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            ViewMode = PickerViewMode.List
        };
        picker.FileTypeFilter.Add("*");

        // Get the window handle for the picker
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var folder = await picker.PickSingleFolderAsync();
        if (folder != null && ViewModel != null)
        {
            ViewModel.OutputFolder = folder.Path;
            ViewModel.AppendLog($"Selected output folder: {folder.Path}");
        }
    }
}
