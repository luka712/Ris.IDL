using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Ris.Idl.Gui.DbModel;
using Ris.Idl.Gui.Models;
using Ris.Idl.Gui.Pages;
using Ris.Idl.Gui.Services;
using Uno.Extensions;

namespace Ris.Idl.Gui.ViewModel;

/// <summary>
/// The convert view model.
/// </summary>
public partial class ConvertPageViewModel : BaseViewModel
{
    private readonly ConversionService _service;
    private readonly ProjectGeneratorService _projectGeneratorService;
    private readonly AppLogger _appLogger;
    
    [ObservableProperty]
    private MainViewModel? _mainViewModel;

    [ObservableProperty] private string _logText = "";
    
    [ObservableProperty] private bool _isBusy;
    
    private ConversionProjectDb? _selectedConvertItem;
    
    [ObservableProperty]
    private bool _hasSelectedConvertItem;

    /// <summary>
    /// The constructor.
    /// </summary>
    public ConvertPageViewModel()
    {
        _service = App.Services.GetService<ConversionService>()!;
        _projectGeneratorService = App.Services.GetService<ProjectGeneratorService>()!;
        _appLogger = App.Services.GetService<AppLogger>()!;

        _appLogger.OnLog += (sender, log) =>
        {
            DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
            {
                LogText += $"{log.Message}\n";
            });
        };
        
        _ = LoadDataAsync();
    }

    /// <summary>
    /// The main view model.
    /// </summary>
    public ConversionProjectDb? SelectedConvertItem
    {
        get => _selectedConvertItem;
        set
        {
            SetProperty(ref _selectedConvertItem, value);
            HasSelectedConvertItem = _selectedConvertItem != null;
        }
    }

    /// <summary>
    /// Loads the data.
    /// </summary>
    public async Task LoadDataAsync()
    {
        var conversions = await _service.GetConversionsAsync();
        DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
        {
            ConvertItems.Clear();
            ConvertItems.AddRange(conversions);
        });
    }
    
    /// <summary>
    /// The list of convert entries.
    /// </summary>
    public ObservableCollection<ConversionProjectDb> ConvertItems { get; } = new();
    
    /// <summary>
    /// Add a new convert entry.
    /// </summary>
    [RelayCommand]
    private void AddConvertItem()
    {
        MainViewModel!.Frame.Navigate(typeof(AddConversionProjectPage), this);
    }

    [RelayCommand]
    private async Task Build()
    {
        if (SelectedConvertItem == null)
        {
            return;
        }
        
        IsBusy = true;
        await _projectGeneratorService.GenerateProjectAsync(SelectedConvertItem);
        
        DispatcherQueue.GetForCurrentThread().TryEnqueue(() => IsBusy = false);
    }
}