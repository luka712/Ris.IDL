namespace Ris.Idl.Core;

/// <summary>
/// Configuration for project generation.
/// </summary>
public class IdlProjectConfiguration
{
    /// <summary>
    /// The name of the project/package.
    /// </summary>
    public string Name { get; set; } = "generated-types";
    
    /// <summary>
    /// The version of the project/package.
    /// </summary>
    public string Version { get; set; } = "1.0.0";
    
    /// <summary>
    /// The description of the project/package.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// The author of the project/package.
    /// </summary>
    public string? Author { get; set; }
    
    /// <summary>
    /// The output directory for the generated project.
    /// </summary>
    public string OutputDirectory { get; set; } = "./generated";
    
    /// <summary>
    /// The generator configuration.
    /// </summary>
    public GeneratorConfig GeneratorConfig { get; set; } = new();
}