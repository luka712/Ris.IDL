using System.Security.Cryptography;
using System.Text;
using Ris.Idl.Symbols;

namespace Ris.Idl.Core;

/// <summary>
/// Generates unique IDs for symbols.
/// </summary>
public class IdGenerator
{
    /// <summary>
    /// Creates ID for a symbol.
    /// </summary>
    /// <param name="namedSymbol">The <see cref="IIdlNamedSymbol"/>.</param>
    /// <returns>The id.</returns>
    public static string CreateId(IIdlNamedSymbol namedSymbol)
    {
        var value = $"{namedSymbol.Namespace}.{namedSymbol.Name}";
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(hash[..8]); // 16 hex chars
    }
}