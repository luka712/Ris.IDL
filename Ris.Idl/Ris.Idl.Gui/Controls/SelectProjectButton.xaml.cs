using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Uno.UI.Xaml;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Ris.Idl.Gui.Controls;

public sealed partial class SelectProjectButton : UserControl
{
    public SelectProjectButton()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// The selected project.
    /// </summary>
    public string? Project
    {
        get => (string)GetValue(ProjectProperty);
        set => SetValue(ProjectProperty, value);
    }

    /// <summary>
    /// The selected project property.
    /// </summary>
    public static readonly DependencyProperty ProjectProperty =
        DependencyProperty.Register(
            nameof(Project),
            typeof(string),
            typeof(SelectProjectButton),
            new PropertyMetadata(""));


    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        _ = PickFileAsync();
    }


    private async Task PickFileAsync()
    {
        var fileOpenPicker = new FileOpenPicker();
        fileOpenPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
        fileOpenPicker.FileTypeFilter.Add(".csproj");
        fileOpenPicker.FileTypeFilter.Add(".sln");

// For Uno.WinUI-based apps
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(fileOpenPicker, hwnd);

        StorageFile pickedFile = await fileOpenPicker.PickSingleFileAsync();
        if (pickedFile != null)
        {
            var filePath = pickedFile.Path;
            Project = filePath;
        }
    }
}