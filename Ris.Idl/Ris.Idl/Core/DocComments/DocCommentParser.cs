using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Ris.Idl.Symbols;
using Ris.Idl.Symbols.DocComment;

namespace Ris.Idl.Core.DocComments;

/// <summary>
/// 
/// </summary>
public class DocCommentParser
{
    // TODO:
    public IdlDocCommentSymbol? GenerateDocComment(ISymbol symbol, string indentation = "")
    {
        var xml = symbol.GetDocumentationCommentXml();

        if (string.IsNullOrWhiteSpace(xml))
        {
            return null;
        }

        try
        {
            var doc = XElement.Parse(xml);
            return GenerateFromXml(doc, indentation);
        }
        catch
        {
            return null;
        }
    }

    // TODO:
    private IdlDocCommentSymbol GenerateFromXml(XElement doc, string indentation)
    {
        IdlDocCommentSymbol? symbol = new();

        // Summary
        symbol.Summary = doc.Element("summary")?.Value.Trim();
        
        // Remarks
        var remarks = doc.Elements("remarks");
        symbol.Remarks = remarks.Select(r => r.Value.Trim()).ToList();
       
        // Parameters
        symbol.Parameters = doc.Elements("param").Select(x => new IdlDocCommentParameter()
        {
            Name = x.Attribute("name")?.Value,
            Description = x.Value.Trim()
        }).ToList();
        
       
        // Returns
        symbol.Returns = doc.Element("returns")?.Value.Trim();
        
        return symbol;
    }
}
