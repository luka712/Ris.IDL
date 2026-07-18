namespace Ris.Idl.Core;

/// <summary>
/// Naming case conventions.
/// </summary>
public enum NamingCase
{
    /// <summary>PascalCase - ThisIsAnExample</summary>
    Pascal,
    /// <summary>camelCase - thisIsAnExample</summary>
    Camel,
    /// <summary>snake_case - this_is_an_example</summary>
    Snake,
    /// <summary>lowercase - thisisanexample</summary>
    Lower,
    /// <summary>UPPERCASE - THISISANEXAMPLE</summary>
    Upper,
    /// <summary>kebab-case - this-is-an-example</summary>
    Kebab
}

/// <summary>
/// Base configuration for code generators.
/// </summary>
public class GeneratorConfig
{
    /// <summary>
    /// The case to use for property names.
    /// </summary>
    public NamingCase PropertyCase { get; set; } = NamingCase.Pascal;
    
    /// <summary>
    /// The case to use for module/namespace paths.
    /// </summary>
    public NamingCase ModuleCase { get; set; } = NamingCase.Pascal;
    
    /// <summary>
    /// Whether to include XML documentation comments.
    /// </summary>
    public bool IncludeDocComments { get; set; } = true;
    
    /// <summary>
    /// Whether to export types (make them public).
    /// </summary>
    public bool ExportTypes { get; set; } = true;
}
