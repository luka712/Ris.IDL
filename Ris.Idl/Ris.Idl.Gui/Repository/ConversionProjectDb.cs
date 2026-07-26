using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ris.Idl.Gui.Repository;

/// <summary>
/// The already created conversions.
/// </summary>
[Table("ConversionProjects")]
[PrimaryKey(nameof(Id))]
public class ConversionProjectDb
{
    /// <summary>
    /// The ID of the conversion.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The file path of the source project.
    /// </summary>
    public string? SourceProject { get; set; }
    
    /// <summary>
    /// The file path of the target project.
    /// </summary>
    public string? TargetFilePath { get; set; }
    
    /// <summary>
    /// The date and time when the conversion was created.
    /// </summary>
    public DateTime Created { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// The date and time when the conversion was last updated.
    /// </summary>
    public DateTime Updated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The type of the project being converted.
    /// </summary>
    public ProjectType ProjectType { get; set; } = ProjectType.FullProject;
    
    /// <summary>
    /// The target language for the conversion.
    /// </summary>
    public TargetLanguage Language { get; set; } = TargetLanguage.TypeScript;
    
    /// <summary>
    /// The tree of the project being converted.
    /// </summary>
    public string? ProjectTree { get; set; } 
}