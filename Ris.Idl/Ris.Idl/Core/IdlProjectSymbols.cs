using System.Text.Json;
using Ris.Idl.Symbols;

namespace Ris.Idl.Core;

/// <summary>
/// All the symbols in a project.
/// </summary>
public class IdlProjectSymbols
{
    /// <summary>
    /// The interfaces in the project.
    /// </summary>
    public IReadOnlyList<IdlInterfaceSymbol> Interfaces { get; set; } = [];

    /// <summary>
    /// The classes in the project.
    /// </summary>
    public IReadOnlyList<IdlClassSymbol> Classes { get; set; } = [];

    /// <summary>
    /// The enums in the project.
    /// </summary>
    public IReadOnlyList<IdlEnumSymbol> Enums { get; set; } = [];

    /// <summary>
    /// The structs in the project.
    /// </summary>
    public List<IdlStructSymbol> Structs { get; set; } = [];

    /// <summary>
    /// Converts the project to JSON representation.
    /// </summary>
    /// <returns>The project as JSON representation.</returns>
    public string ToJson() => JsonSerializer.Serialize(this);

    /// TODO
    public static async Task<IdlProjectSymbols> FromJson(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
        {
            throw new FileNotFoundException("File not found", jsonFilePath);
        }
        
        var json = await File.ReadAllTextAsync(jsonFilePath);

        try
        {
            return JsonSerializer.Deserialize<IdlProjectSymbols>(json)!;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to parse JSON file: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets all symbols in the project.
    /// </summary>
    /// <returns>The list of all the symbols.</returns>
    public List<IIdlNamedSymbol> GetAllSymbols()
    {
        var symbols = new List<IIdlNamedSymbol>();

        if (Interfaces is not null)
        {
            symbols.AddRange(Interfaces);
        }

        if (Classes is not null)
        {
            symbols.AddRange(Classes);
        }

        if (Enums is not null)
        {
            symbols.AddRange(Enums);
        }

        if (Structs is not null)
        {
            symbols.AddRange(Structs);
        }
        
        return symbols;
    }
}