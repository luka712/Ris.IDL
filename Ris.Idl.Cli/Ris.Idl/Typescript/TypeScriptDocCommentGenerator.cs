using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace Ris.Idl.Typescript;

public class TypeScriptDocCommentGenerator
{
    private const string EMPTY = "";
    private const char NEW_LINE_CHAR = '\n';
    
    public string GetDocComment(ISymbol symbol)
    {
        bool indent = symbol.Kind == SymbolKind.Method || symbol.Kind == SymbolKind.Property;
        
        var xml = symbol.GetDocumentationCommentXml();

        if (string.IsNullOrWhiteSpace(xml))
        {
            return EMPTY;
        }

        var doc = XElement.Parse(xml);

        var summary = doc.Element("summary")?
            .Value
            .Trim();

        if (string.IsNullOrWhiteSpace(summary))
        {
            return EMPTY;
        }

        var sb = new StringBuilder();
        
        sb.AppendLine("/**");
        foreach (var line in summary.Split(NEW_LINE_CHAR))
        {
            sb.AppendLine($" * {line}");
        }

        sb.Append(" */"); 
        
        return sb.ToString();
    }
}