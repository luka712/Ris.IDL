using Windows.Storage.Pickers;

namespace Ris.Idl.Gui.Controls;

public class LabelFilePicker : Control
{
    private Button? _button;
    
    public LabelFilePicker()
    {
        DefaultStyleKey = typeof(LabelFilePicker);
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
            typeof(LabelFilePicker),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// The file path.
    /// </summary>
    public string? FilePath
    {
        get => (string)GetValue(FilePathProperty);
        set => SetValue(FilePathProperty, value);
    }

    /// <summary>
    /// The file path property.
    /// </summary>
    public static readonly DependencyProperty FilePathProperty =
        DependencyProperty.Register(
            nameof(FilePath),
            typeof(string),
            typeof(LabelFilePicker),
            new PropertyMetadata(default(string)));

    /// <summary>
    /// The file extensions.
    /// </summary>
    public string[] FileExtensions
    {
        get => (string[])GetValue(FileExtensionsProperty);
        set => SetValue(FileExtensionsProperty, value);
    }

    public static readonly DependencyProperty FileExtensionsProperty =
        DependencyProperty.Register(
            nameof(FileExtensions),
            typeof(string[]),
            typeof(LabelFilePicker),
            new PropertyMetadata(new[] { "*" }));

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
        var fileOpenPicker = new FileOpenPicker();
        fileOpenPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;

        foreach (var extension in FileExtensions)
        {
            fileOpenPicker.FileTypeFilter.Add(extension);
        }

        // For Uno.WinUI-based apps
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(fileOpenPicker, hwnd);

        StorageFile pickedFile = await fileOpenPicker.PickSingleFileAsync();
        if (pickedFile != null)
        {
            FilePath = pickedFile.Path;
        }
    }
}