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
    /// The case to use for enum.
    /// </summary>
    public NamingCase EnumCase { get; set; } = NamingCase.Pascal;
    
    /// <summary>
    /// The case to use for enum keys.
    /// </summary>
    public NamingCase EnumKeyCase { get; set; } = NamingCase.Upper;
    
    /// <summary>
    /// The case to use for event names.
    /// </summary>
    public NamingCase EventCase { get; set; } = NamingCase.Pascal;
    
    /// <summary>
    /// Whether to add a newline between properties.
    /// </summary>
    public bool AddNewlineBetweenProperties { get; set; } = true;
    
    /// <summary>
    /// Whether to add a newline between methods.
    /// </summary>
    public bool AddNewlineBetweenMethods { get; set; } = true;
    
    /// <summary>
    /// Whether to add a newline between enum values.
    /// </summary>
    public bool AddNewLinesBetweenEnumValues { get; set; } = true;

    
    /// <summary>
    /// Whether to include XML documentation comments.
    /// </summary>
    public bool IncludeDocComments { get; set; } = true;
    
    /// <summary>
    /// Whether to export types (make them public).
    /// </summary>
    public bool ExportTypes { get; set; } = true;
}
