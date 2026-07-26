using Microsoft.EntityFrameworkCore;

namespace Ris.Idl.Gui.Repository;

/// <summary>
/// The database context for the RIS Idl GUI application.
/// </summary>
public sealed class RisIdlDbContext : DbContext
{
    /// <summary>
    /// The IdlConversions table.
    /// </summary>
    public DbSet<ConversionProjectDb> IdlConversions { get; set; }
    
    /// <summary>
    /// The ConversionSettings table.
    /// </summary>
    public DbSet<ConversionSettingsDb> ConversionSettings { get; set; }

    /// <summary>
    /// The constructor.
    /// </summary>
    public RisIdlDbContext()
    {
        Database.EnsureCreated();
    }
    
    /// <inheritdoc/>
    protected override void OnConfiguring(
        DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=database.db");
    }
}