using Ris.Idl;
using Ris.Idl.TypeScript;
using Ris.Idl.TypeScript.Configuration;

// Example usage of Ris.Idl to generate TypeScript from C# projects

if (args.Length == 0)
{
    Console.WriteLine("Ris.Idl - C# to TypeScript Code Generator");
    Console.WriteLine();
    Console.WriteLine("Usage: Ris.Idl.Cli <project.csproj> [output-directory]");
    Console.WriteLine();
    Console.WriteLine("Example:");
    Console.WriteLine("  Ris.Idl.Cli MyProject.csproj ./generated-types");
    return;
}

var projectPath = args[0];
var outputDirectory = args.Length > 1 ? args[1] : "./generated";

if (!File.Exists(projectPath))
{
    Console.Error.WriteLine($"Error: Project file not found: {projectPath}");
    return;
}

Console.WriteLine($"Loading project: {projectPath}");

var loader = new ProjectLoader();
var configuration = new TypeScriptProjectConfiguration
{
    Name = Path.GetFileNameWithoutExtension(projectPath).ToLowerInvariant() + "-types",
    Version = "1.0.0",
    Description = $"TypeScript types generated from {Path.GetFileName(projectPath)}",
    OutputDirectory = outputDirectory
};

try
{
    var project = await loader.GenerateProjectAsync(projectPath, configuration);
    
    Console.WriteLine($"Generated {project.Files.Count} TypeScript file(s):");
    foreach (var file in project.Files)
    {
        Console.WriteLine($"  - {file.RelativePath}");
    }
    
    Console.WriteLine();
    Console.WriteLine("Project files:");
    foreach (var (path, _) in project.ProjectFiles)
    {
        Console.WriteLine($"  - {path}");
    }
    
    // Write to disk
    var projectGenerator = new TypeScriptProjectGenerator();
    await projectGenerator.WriteProjectAsync(project);
    
    Console.WriteLine();
    Console.WriteLine($"Output written to: {Path.GetFullPath(outputDirectory)}");
    Console.WriteLine();
    Console.WriteLine("To build the TypeScript project:");
    Console.WriteLine($"  cd {outputDirectory}");
    Console.WriteLine("  npm install");
    Console.WriteLine("  npm run build");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
}
