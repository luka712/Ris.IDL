using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ris.Idl.Gui.Repository;

[Table("ConversionSettings")]
[PrimaryKey(nameof(Id))]
public class ConversionSettingsDb
{
    /// <summary>
    /// The ID of the conversion settings.
    /// </summary>
    public int Id { get; set; }
}