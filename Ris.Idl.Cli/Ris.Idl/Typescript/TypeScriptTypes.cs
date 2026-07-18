using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ris.Idl.Typescript;

public class TypeScriptTypes
{
    public string ConvertType(ITypeSymbol type)
    {
        return type.Name switch
        {
            "string" => "string",
            "int" => "number",
            "long" => "number",
            "float" => "number",
            "double" => "number",
            "decimal" => "number",
            "bool" => "boolean",

            "DateTime" => "string",
            "Guid" => "string",

            _ => type.Name
        };
    }
}