using Ris.Idl.Core;

namespace Ris.Idl.TypeScript;

/// <summary>
/// TypeScript-specific generator configuration.
/// </summary>
public class TypeScriptConfig : GeneratorConfig
{
    public TypeScriptConfig()
    {
        // TypeScript properties are typically camelCase
        PropertyCase = NamingCase.Camel;
        
        // Module paths are typically lowercase
        ModuleCase = NamingCase.Lower;
    }
    
    /// <summary>
    /// Whether to generate readonly properties for get-only C# properties.
    /// </summary>
    public bool UseReadonlyModifier { get; set; } = true;
    
    /// <summary>
    /// Whether to use 'interface' for C# classes (DTOs) instead of 'class'.
    /// </summary>
    public bool UseInterfaceForClasses { get; set; } = true;
    
    /// <summary>
    /// The indentation string to use (default is 2 spaces).
    /// </summary>
    public string Indentation { get; set; } = "  ";
    
    /// <summary>
    /// Whether to add a newline between properties.
    /// </summary>
    public bool AddNewlineBetweenProperties { get; set; } = true;
}
