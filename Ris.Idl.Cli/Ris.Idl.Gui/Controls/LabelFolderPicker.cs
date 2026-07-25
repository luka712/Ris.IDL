using Windows.Storage.Pickers;

namespace Ris.Idl.Gui.Controls;

public class LabelFolderPicker : Control
{
    private Button? _button;
    
    public LabelFolderPicker()
    {
        DefaultStyleKey = typeof(LabelFolderPicker);
    }

    /// <summary>
    /// The label.
    /// </summary>
    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    /// <summary>
    /// The label property.
    /// </summary>
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(
            nameof(Label),
            typeof(string),
            typeof(LabelFolderPicker),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// The file path.
    /// </summary>
    public string? FolderPath
    {
        get => (string)GetValue(FolderPathProperty);
        set => SetValue(FolderPathProperty, value);
    }

    /// <summary>
    /// The file path property.
    /// </summary>
    public static readonly DependencyProperty FolderPathProperty =
        DependencyProperty.Register(
            nameof(FolderPath),
            typeof(string),
            typeof(LabelFolderPicker),
            new PropertyMetadata(default(string)));
    

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        
        _button = GetTemplateChild("PART_Button") as Button;
        _button?.Click += OnPickFile;
    }

    private void OnPickFile(object sender, RoutedEventArgs e)
    {
        _ = PickFileAsync();
    }

    private async Task PickFileAsync()
    {
        var folderPicker = new FolderPicker();
        folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
        
        // For Uno.WinUI-based apps
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

        var pickedFolder = await folderPicker.PickSingleFolderAsync();
        if (pickedFolder != null)
        {
            FolderPath = pickedFolder.Path;
        }
    }
}