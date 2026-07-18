using System.Text;
using System.Text.RegularExpressions;

namespace Ris.Idl.Core;

/// <summary>
/// Helper class for naming conventions and transformations.
/// </summary>
public static partial class NamingHelper
{
    /// <summary>
    /// Formats a name according to the specified case convention.
    /// </summary>
    /// <param name="name">The name to format.</param>
    /// <param name="namingCase">The target case convention.</param>
    /// <returns>The formatted name.</returns>
    public static string FormatName(string name, NamingCase namingCase)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        return namingCase switch
        {
            NamingCase.Pascal => ToPascalCase(name),
            NamingCase.Camel => ToCamelCase(name),
            NamingCase.Snake => ToSnakeCase(name),
            NamingCase.Lower => name.ToLowerInvariant(),
            NamingCase.Upper => name.ToUpperInvariant(),
            NamingCase.Kebab => ToKebabCase(name),
            _ => name
        };
    }

    /// <summary>
    /// Converts a C# namespace to a module path.
    /// </summary>
    /// <param name="namespace">The namespace to convert.</param>
    /// <param name="namingCase">The target case convention.</param>
    /// <param name="separator">The path separator (default is '/').</param>
    /// <returns>The module path.</returns>
    public static string NamespaceToModulePath(string @namespace, NamingCase namingCase, char separator = '/')
    {
        if (string.IsNullOrEmpty(@namespace))
            return @namespace;

        var parts = @namespace.Split('.');
        var formattedParts = parts.Select(p => FormatName(p, namingCase));
        return string.Join(separator, formattedParts);
    }

    /// <summary>
    /// Converts a name to PascalCase.
    /// </summary>
    public static string ToPascalCase(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        // If already PascalCase, return as-is
        if (char.IsUpper(name[0]) && !name.Contains('_') && !name.Contains('-'))
            return name;

        var words = SplitIntoWords(name);
        return string.Concat(words.Select(w => char.ToUpperInvariant(w[0]) + w[1..].ToLowerInvariant()));
    }

    /// <summary>
    /// Converts a name to camelCase.
    /// </summary>
    public static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        var pascal = ToPascalCase(name);
        return char.ToLowerInvariant(pascal[0]) + pascal[1..];
    }

    /// <summary>
    /// Converts a name to snake_case.
    /// </summary>
    public static string ToSnakeCase(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        var words = SplitIntoWords(name);
        return string.Join('_', words.Select(w => w.ToLowerInvariant()));
    }

    /// <summary>
    /// Converts a name to kebab-case.
    /// </summary>
    public static string ToKebabCase(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        var words = SplitIntoWords(name);
        return string.Join('-', words.Select(w => w.ToLowerInvariant()));
    }

    /// <summary>
    /// Splits a name into words based on common naming conventions.
    /// </summary>
    private static string[] SplitIntoWords(string name)
    {
        // Handle snake_case and kebab-case
        if (name.Contains('_'))
            return name.Split('_', StringSplitOptions.RemoveEmptyEntries);
        
        if (name.Contains('-'))
            return name.Split('-', StringSplitOptions.RemoveEmptyEntries);

        // Handle PascalCase and camelCase
        var words = new List<string>();
        var currentWord = new StringBuilder();

        foreach (var c in name)
        {
            if (char.IsUpper(c) && currentWord.Length > 0)
            {
                words.Add(currentWord.ToString());
                currentWord.Clear();
            }
            currentWord.Append(c);
        }

        if (currentWord.Length > 0)
            words.Add(currentWord.ToString());

        return words.ToArray();
    }

    /// <summary>
    /// Sanitizes a file name by replacing invalid characters.
    /// </summary>
    /// <param name="fileName">The file name to sanitize.</param>
    /// <returns>The sanitized file name.</returns>
    public static string SanitizeFileName(string fileName)
    {
        // Replace dots with underscores (except for extension)
        var lastDot = fileName.LastIndexOf('.');
        if (lastDot > 0)
        {
            var nameWithoutExt = fileName[..lastDot].Replace('.', '_');
            var ext = fileName[lastDot..];
            return nameWithoutExt + ext;
        }
        
        return fileName.Replace('.', '_');
    }
}
