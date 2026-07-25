using Ris.Idl.Gui.DbModel;
using Ris.Idl.TypeScript;
using Ris.Idl.TypeScript.Configuration;
using Uno.Logging;

namespace Ris.Idl.Gui.Services;

public class ProjectGeneratorService
{
    private readonly ProjectLoader _loader;
    private readonly AppLogger _logger;
    
    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="loader">The <see cref="ProjectLoader"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public ProjectGeneratorService(ProjectLoader loader, AppLogger logger)
    {
        _loader = loader;
        _logger = logger;
    }

    public async Task GenerateProjectAsync(ConversionProjectDb conversionProject)
    {
        if (conversionProject.ProjectType == ProjectType.FullProject)
        {
            await GenerateFullProjectAsync(conversionProject);
        }
    }
    
      private async Task GenerateFullProjectAsync(ConversionProjectDb conversionProject)
      {
          var projectPath = conversionProject.SourceProject!;
          var outputFolder = conversionProject.TargetFilePath!;
        
        var configuration = new TypeScriptProjectConfiguration
        {
            Name = projectPath.Split('/').Last(),
            Version = "1.0.0",
            Description = $"TypeScript types generated from {Path.GetFileName(projectPath)}",
            OutputDirectory = outputFolder
        };
        
        _logger.Info($"Generating project: {projectPath}...");

        var project = await  _loader.GenerateProjectAsync(projectPath!, configuration);

        _logger.Info($"Generated {project.Files.Count} TypeScript file(s):");
        foreach (var file in project.Files)
        {
            _logger.Info($"  - {file.RelativePath}");
        }

        _logger.Info("Project files:");
        foreach (var (path, _) in project.ProjectFiles)
        {
            _logger.Info($"  - {path}");
        }

        // Write to disk
        var projectGenerator = new TypeScriptProjectGenerator();
        await projectGenerator.WriteProjectAsync(project);

        _logger.Info($"Output written to: {outputFolder}");
        // StatusText = $"✓ Successfully generated {project.Files.Count} TypeScript file(s)";
        // StatusColor = "Green";
    }

    // private async Task GenerateIdlOnlyAsync(ProjectLoader loader, string projectName)
    // {
    //     var configuration = new TypeScriptIdlProjectConfiguration
    //     {
    //         Name = projectName,
    //         Version = "1.0.0",
    //         Description = $"TypeScript interfaces generated from {Path.GetFileName(ProjectPath)}",
    //         OutputDirectory = OutputFolder!
    //     };
    //
    //     // Load files with IDL configuration (no src folder prefix)
    //     var files = await Task.Run(() => loader.LoadProjectAsync(ProjectPath!, configuration.GeneratorConfig));
    //
    //     AppendLog($"Generated {files.Count} TypeScript interface(s):");
    //     foreach (var file in files)
    //     {
    //         AppendLog($"  - {file.RelativePath}");
    //     }
    //
    //     // Create IDL project and write to disk
    //     var projectGenerator = new TypeScriptProjectGenerator();
    //     var project = await projectGenerator.GenerateIdlProjectAsync(configuration, files);
    //     await Task.Run(() => projectGenerator.WriteProjectAsync(project));
    //
    //     AppendLog($"Output written to: {OutputFolder}");
    //     StatusText = $"✓ Successfully generated {files.Count} TypeScript interface(s)";
    //     StatusColor = "Green";
    // }
}