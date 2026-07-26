using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Ris.Idl.Gui.Repository;
using Ris.Idl.Gui.Services;

namespace Ris.Idl.Gui.ViewModel;

/// <summary>
/// The convert project view model.
/// </summary>
public partial class EditConversionProjectViewModel : ObservableValidator
{
    private readonly ConversionService _service;

    [ObservableProperty] private MainViewModel? _mainViewModel;

    [Required(ErrorMessage = "The source project is required.")]
    [CustomValidation(typeof(EditConversionProjectViewModel), nameof(ValidateSourceProject))]
    [ObservableProperty]
    private string? _sourceProject;

    [Required(ErrorMessage = "The target path is required.")]
    [CustomValidation(typeof(EditConversionProjectViewModel), nameof(ValidateTargetPath))]
    [ObservableProperty]
    private string? _targetPath;

    [ObservableProperty] private TargetLanguage _selectedTargetLanguage;

    [ObservableProperty] private ProjectType _selectedProjectType;

    [ObservableProperty] private bool _hasInfo;

    [ObservableProperty] private string? _infoText;

     private ConversionProjectDb? _project;

    public EditConversionProjectViewModel()
    {
        _service = App.Services.GetService<ConversionService>()!;
        ErrorsChanged += (sender, args) => OnPropertyChanged(nameof(HasErrors));
    }

    public ConversionProjectDb? Project
    {
        get => _project;
        set
        {
            if (value == null)
            {
                return;
            }
            
            SourceProject = value.SourceProject;
            TargetPath = value.TargetFilePath;
            SelectedProjectType = value.ProjectType;
            SelectedTargetLanguage = value.Language;
            
            SetProperty(ref _project, value);   
        }
    }

    /// <summary>
    /// The list of target languages.
    /// </summary>
    public IReadOnlyList<TargetLanguage> TargetLanguages { get; } = [TargetLanguage.TypeScript];

    /// <summary>
    /// The list of project types.
    /// </summary>
    public IReadOnlyList<ProjectType> ProjectTypes { get; } = [ProjectType.FullProject, ProjectType.IdlOnly];

    /// <summary>
    /// The list of error messages.
    /// </summary>
    public ObservableCollection<string> ErrorMessages { get; } = new();

    public static ValidationResult ValidateSourceProject(string name, ValidationContext context)
    {
        EditConversionProjectViewModel instance = (EditConversionProjectViewModel)context.ObjectInstance;

        List<string> members = [];
        if (context.MemberName != null)
        {
            members.Add(context.MemberName);
        }

        var sourceProject = instance.SourceProject;

        if (!File.Exists(sourceProject))
        {
            return new("The source project is not selected.", members);
        }

        if (!(sourceProject.EndsWith(".csproj") || sourceProject.EndsWith(".sln")))
        {
            return new("The source project must be a .csproj or .sln file.", members);
        }

        return ValidationResult.Success!;
    }

    public static ValidationResult ValidateTargetPath(string name, ValidationContext context)
    {
        EditConversionProjectViewModel instance = (EditConversionProjectViewModel)context.ObjectInstance;
        List<string> members = [];
        if (context.MemberName != null)
        {
            members.Add(context.MemberName);
        }

        var targetPath = instance.TargetPath;

        if (!Directory.Exists(targetPath))
        {
            return new("The target path is invalid.", members);
        }

        return ValidationResult.Success!;
    }

    [RelayCommand]
    public async Task UpdateConversionAsync()
    {
        ErrorMessages.Clear();
        ValidateAllProperties();

        if (HasErrors)
        {
            var errors = GetErrors().Where(x => !string.IsNullOrEmpty(x.ErrorMessage));
            foreach (var error in errors)
            {
                var memberNames = string.Join(", ", error.MemberNames);
                if (!string.IsNullOrEmpty(memberNames))
                {
                    DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
                        ErrorMessages.Add($"{memberNames}: {error.ErrorMessage}"));
                }
                else
                {
                    DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
                        ErrorMessages.Add(error.ErrorMessage!));
                }
            }

            return;
        }

        try
        {
            await _service.UpdateConversionAsync(Project, this);
        }
        catch (Exception ex)
        {
            ErrorMessages.Add(ex.Message);
            return;
        }

        DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
        {
            HasInfo = true;
            InfoText = "Conversion project updated successfully!";
        });
    }

    [RelayCommand]
    public void Cancel()
    {
        MainViewModel!.Frame.GoBack();
    }
}