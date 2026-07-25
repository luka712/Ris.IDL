using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ris.Idl.Gui.Models;
using Ris.Idl.TypeScript;
using Ris.Idl.TypeScript.Configuration;

namespace Ris.Idl.Gui.ViewModel;

public partial class MainViewModel : BaseViewModel
{
    private readonly StringBuilder _logBuilder = new();
    
    

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(GenerateCommand))]
    private string? _projectPath;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(GenerateCommand))]
    private string? _outputFolder;

    [ObservableProperty] private ProjectType _selectedProjectType = ProjectType.FullProject;

    [ObservableProperty] private string _statusText = string.Empty;

    [ObservableProperty] private string _statusColor = "Gray";

    [ObservableProperty] private bool _isBusy;

    [ObservableProperty] private string _logText = "Output log will appear here...";

    /// <summary>
    /// Available project types for the ComboBox.
    /// </summary>
    public ProjectType[] ProjectTypes { get; } = Enum.GetValues<ProjectType>();

    /// <summary>
    /// Gets a display-friendly name for a project type.
    /// </summary>
    public static string GetProjectTypeDisplayName(ProjectType type) => type switch
    {
        ProjectType.FullProject => "Full Project (with package.json, tsconfig.json)",
        ProjectType.IdlOnly => "IDL Only (interfaces without project structure)",
        _ => type.ToString()
    };

    public bool CanGenerate => !string.IsNullOrEmpty(ProjectPath) &&
                               !string.IsNullOrEmpty(OutputFolder) &&
                               !IsBusy;

    /// <summary>
    /// The main frame.
    /// </summary>
    public Frame Frame { get; set; } = null!;

    [RelayCommand(CanExecute = nameof(CanGenerate))]
    private async Task GenerateAsync()
    {
        if (string.IsNullOrEmpty(ProjectPath) || string.IsNullOrEmpty(OutputFolder))
        {
            return;
        }

        IsBusy = true;
        StatusText = "Generating TypeScript...";
        StatusColor = "Gray";

        try
        {
            AppendLog("Starting generation...");
            AppendLog($"Input: {ProjectPath}");
            AppendLog($"Output: {OutputFolder}");
            AppendLog($"Project Type: {GetProjectTypeDisplayName(SelectedProjectType)}");

            var loader = new ProjectLoader();
            var projectName = Path.GetFileNameWithoutExtension(ProjectPath).ToLowerInvariant() + "-types";

            if (SelectedProjectType == ProjectType.FullProject)
            {
                await GenerateFullProjectAsync(loader, projectName);
            }
            else
            {
                await GenerateIdlOnlyAsync(loader, projectName);
            }

            AppendLog("Generation complete!");
        }
        catch (Exception ex)
        {
            AppendLog($"Error: {ex.Message}");
            StatusText = $"✗ Error: {ex.Message}";
            StatusColor = "Red";
        }
        finally
        {
            IsBusy = false;
            GenerateCommand.NotifyCanExecuteChanged();
        }
    }

    private async Task GenerateFullProjectAsync(ProjectLoader loader, string projectName)
    {
        var configuration = new TypeScriptProjectConfiguration
        {
            Name = projectName,
            Version = "1.0.0",
            Description = $"TypeScript types generated from {Path.GetFileName(ProjectPath)}",
            OutputDirectory = OutputFolder!
        };

        var project = await Task.Run(() => loader.GenerateProjectAsync(ProjectPath!, configuration));

        AppendLog($"Generated {project.Files.Count} TypeScript file(s):");
        foreach (var file in project.Files)
        {
            AppendLog($"  - {file.RelativePath}");
        }

        AppendLog("Project files:");
        foreach (var (path, _) in project.ProjectFiles)
        {
            AppendLog($"  - {path}");
        }

        // Write to disk
        var projectGenerator = new TypeScriptProjectGenerator();
        await Task.Run(() => projectGenerator.WriteProjectAsync(project));

        AppendLog($"Output written to: {OutputFolder}");
        StatusText = $"✓ Successfully generated {project.Files.Count} TypeScript file(s)";
        StatusColor = "Green";
    }

    private async Task GenerateIdlOnlyAsync(ProjectLoader loader, string projectName)
    {
        var configuration = new TypeScriptIdlProjectConfiguration
        {
            Name = projectName,
            Version = "1.0.0",
            Description = $"TypeScript interfaces generated from {Path.GetFileName(ProjectPath)}",
            OutputDirectory = OutputFolder!
        };

        // Load files with IDL configuration (no src folder prefix)
        var files = await Task.Run(() => loader.LoadProjectAsync(ProjectPath!, configuration.GeneratorConfig));

        AppendLog($"Generated {files.Count} TypeScript interface(s):");
        foreach (var file in files)
        {
            AppendLog($"  - {file.RelativePath}");
        }

        // Create IDL project and write to disk
        var projectGenerator = new TypeScriptProjectGenerator();
        var project = await projectGenerator.GenerateIdlProjectAsync(configuration, files);
        await Task.Run(() => projectGenerator.WriteProjectAsync(project));

        AppendLog($"Output written to: {OutputFolder}");
        StatusText = $"✓ Successfully generated {files.Count} TypeScript interface(s)";
        StatusColor = "Green";
    }

    public void AppendLog(string message)
    {
        _logBuilder.AppendLine($"[{DateTime.Now:HH:mm:ss}] {message}");
        LogText = _logBuilder.ToString();
    }

    partial void OnIsBusyChanged(bool value)
    {
        GenerateCommand.NotifyCanExecuteChanged();
    }

    internal void LoadSettings()
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        ProjectPath = localSettings.Values["ProjectPath"]?.ToString();
        OutputFolder = localSettings.Values["OutputFolder"]?.ToString();

        var type = localSettings.Values["SelectedProjectType"]?.ToString();
        if (type != null)
        {
            if (Enum.TryParse(type, out ProjectType projectType))
            {
                SelectedProjectType = projectType;
            }
        }
    }

    internal void SaveSettings()
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        localSettings.Values["ProjectPath"] = ProjectPath;
        localSettings.Values["OutputFolder"] = OutputFolder;
        localSettings.Values["SelectedProjectType"] = SelectedProjectType.ToString();
    }
}