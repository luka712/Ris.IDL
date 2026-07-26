namespace Ris.Idl.Gui.Models;

/// <summary>
/// Defines the type of TypeScript project to generate.
/// </summary>
public enum ProjectType
{
    /// <summary>
    /// Full TypeScript project with package.json, tsconfig.json, and src folder structure.
    /// </summary>
    FullProject,
    
    /// <summary>
    /// IDL-only generation - interfaces only without project structure.
    /// Files are generated directly in the output folder without src subfolder.
    /// </summary>
    IdlOnly
}
