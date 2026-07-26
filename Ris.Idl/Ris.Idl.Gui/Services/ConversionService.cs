using Microsoft.EntityFrameworkCore;
using Ris.Idl.Gui.Repository;
using Ris.Idl.Gui.ViewModel;

namespace Ris.Idl.Gui.Services;

/// <summary>
/// The conversion service.
/// </summary>
public class ConversionService
{
    private readonly RisIdlDbContext _dbContext;

    /// <summary>
    /// Creates a new conversion service.
    /// </summary>
    /// <param name="dbContext">The database context.</param>  
    public ConversionService(RisIdlDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    /// <summary>
    /// Add a conversion.
    /// </summary>
    /// <param name="conversionProjectVm">The conversion project view model.</param>   
    public Task AddConversionAsync(AddConversionProjectViewModel conversionProjectVm)
    {
        var conversionDb = new ConversionProjectDb();
        conversionDb.SourceProject = conversionProjectVm.SourceProject;
        conversionDb.TargetFilePath = conversionProjectVm.TargetPath;
        conversionDb.Language = conversionProjectVm.SelectedTargetLanguage;
        conversionDb.ProjectType = conversionProjectVm.SelectedProjectType;
        
        _dbContext.Add(conversionDb);
        return _dbContext.SaveChangesAsync();
    }
    
    /// <summary>
    /// Add a conversion.
    /// </summary>
    /// <param name="conversionProjectVm">The conversion project view model.</param>   
    public async Task UpdateConversionAsync(ConversionProjectDb editedProject, EditConversionProjectViewModel conversionProjectVm)
    {
        var model = await _dbContext.IdlConversions.FindAsync(editedProject.Id);
        model!.SourceProject = conversionProjectVm.SourceProject;
        model.TargetFilePath = conversionProjectVm.TargetPath;
        model.Language = conversionProjectVm.SelectedTargetLanguage;
        model.ProjectType = conversionProjectVm.SelectedProjectType;
        model.Updated = DateTime.Now;

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Get all conversions.
    /// </summary>
    /// <returns>The list of all conversions.</returns>
    public async Task<IReadOnlyList<ConversionProjectDb>> GetConversionsAsync()
    {
        var list = await _dbContext.IdlConversions.ToListAsync();
        return list;
    }
}