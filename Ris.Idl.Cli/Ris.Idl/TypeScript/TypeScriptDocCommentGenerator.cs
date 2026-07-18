using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace Ris.Idl.TypeScript;

/// <summary>
/// Generates JSDoc comments from C# XML documentation.
/// </summary>
public class TypeScriptDocCommentGenerator
{
    /// <summary>
    /// Generates a JSDoc comment from a symbol's XML documentation.
    /// </summary>
    /// <param name="symbol">The symbol to get documentation for.</param>
    /// <param name="indentation">The indentation to use.</param>
    /// <returns>The JSDoc comment string, or empty if no documentation.</returns>
    public string GenerateDocComment(ISymbol symbol, string indentation = "")
    {
        var xml = symbol.GetDocumentationCommentXml();

        if (string.IsNullOrWhiteSpace(xml))
            return string.Empty;

        try
        {
            var doc = XElement.Parse(xml);
            return GenerateFromXml(doc, indentation);
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Generates JSDoc from parsed XML documentation.
    /// </summary>
    private string GenerateFromXml(XElement doc, string indentation)
    {
        var sb = new StringBuilder();
        var hasContent = false;

        // Summary
        var summary = doc.Element("summary")?.Value.Trim();
        if (!string.IsNullOrWhiteSpace(summary))
        {
            hasContent = true;
            sb.AppendLine($"{indentation}/**");
            foreach (var line in NormalizeText(summary))
            {
                sb.AppendLine($"{indentation} * {line}");
            }
        }

        // Remarks
        var remarks = doc.Element("remarks")?.Value.Trim();
        if (!string.IsNullOrWhiteSpace(remarks))
        {
            if (!hasContent)
            {
                sb.AppendLine($"{indentation}/**");
                hasContent = true;
            }
            sb.AppendLine($"{indentation} *");
            sb.AppendLine($"{indentation} * @remarks");
            foreach (var line in NormalizeText(remarks))
            {
                sb.AppendLine($"{indentation} * {line}");
            }
        }

        // Parameters
        foreach (var param in doc.Elements("param"))
        {
            var paramName = param.Attribute("name")?.Value;
            var paramDesc = param.Value.Trim();
            if (!string.IsNullOrWhiteSpace(paramName))
            {
                if (!hasContent)
                {
                    sb.AppendLine($"{indentation}/**");
                    hasContent = true;
                }
                var desc = string.IsNullOrWhiteSpace(paramDesc) ? "" : $" - {NormalizeText(paramDesc).FirstOrDefault() ?? ""}";
                sb.AppendLine($"{indentation} * @param {paramName}{desc}");
            }
        }

        // Returns
        var returns = doc.Element("returns")?.Value.Trim();
        if (!string.IsNullOrWhiteSpace(returns))
        {
            if (!hasContent)
            {
                sb.AppendLine($"{indentation}/**");
                hasContent = true;
            }
            sb.AppendLine($"{indentation} * @returns {NormalizeText(returns).FirstOrDefault() ?? ""}");
        }

        // Example
        var example = doc.Element("example")?.Value.Trim();
        if (!string.IsNullOrWhiteSpace(example))
        {
            if (!hasContent)
            {
                sb.AppendLine($"{indentation}/**");
                hasContent = true;
            }
            sb.AppendLine($"{indentation} *");
            sb.AppendLine($"{indentation} * @example");
            sb.AppendLine($"{indentation} * ```typescript");
            foreach (var line in NormalizeText(example))
            {
                sb.AppendLine($"{indentation} * {line}");
            }
            sb.AppendLine($"{indentation} * ```");
        }

        // Exceptions
        foreach (var exception in doc.Elements("exception"))
        {
            var exceptionType = exception.Attribute("cref")?.Value;
            var exceptionDesc = exception.Value.Trim();
            if (!string.IsNullOrWhiteSpace(exceptionType))
            {
                if (!hasContent)
                {
                    sb.AppendLine($"{indentation}/**");
                    hasContent = true;
                }
                // Extract just the type name from cref
                var typeName = exceptionType.Split(':').LastOrDefault()?.Split('.').LastOrDefault() ?? exceptionType;
                var desc = string.IsNullOrWhiteSpace(exceptionDesc) ? "" : $" - {NormalizeText(exceptionDesc).FirstOrDefault() ?? ""}";
                sb.AppendLine($"{indentation} * @throws {{{typeName}}}{desc}");
            }
        }

        if (hasContent)
        {
            sb.Append($"{indentation} */");
            return sb.ToString();
        }

        return string.Empty;
    }

    /// <summary>
    /// Normalizes text by trimming and splitting into lines.
    /// </summary>
    private static IEnumerable<string> NormalizeText(string text)
    {
        return text
            .Split('\n')
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrEmpty(line));
    }
}
