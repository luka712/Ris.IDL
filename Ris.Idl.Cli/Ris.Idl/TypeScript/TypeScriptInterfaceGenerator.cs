using System.Text;
using Microsoft.CodeAnalysis;
using Ris.Idl.Core;

namespace Ris.Idl.TypeScript;

/// <summary>
/// Generates TypeScript interfaces from C# interfaces.
/// </summary>
public class TypeScriptInterfaceGenerator : ITypeGenerator
{
    private readonly TypeScriptTypeMapper _typeMapper = new();
    private readonly TypeScriptDocCommentGenerator _docGenerator = new();

    /// <inheritdoc />
    public bool CanGenerate(INamedTypeSymbol type)
    {
        return type.TypeKind == TypeKind.Interface;
    }

    /// <inheritdoc />
    public IGeneratedFile Generate(INamedTypeSymbol type, GeneratorConfig? config = null)
    {
        var tsConfig = config as TypeScriptConfig ?? new TypeScriptConfig();
        
        if (type.TypeKind != TypeKind.Interface)
        {
            throw new ArgumentException($"Expected interface, got {type.TypeKind}", nameof(type));
        }

        var sb = new StringBuilder();
        var indent = tsConfig.Indentation;

        // Generate doc comment for interface
        if (tsConfig.IncludeDocComments)
        {
            var docComment = _docGenerator.GenerateDocComment(type);
            if (!string.IsNullOrEmpty(docComment))
            {
                sb.AppendLine(docComment);
            }
        }

        // Interface declaration
        var exportKeyword = tsConfig.ExportTypes ? "export " : "";
        sb.AppendLine($"{exportKeyword}interface {type.Name} {{");

        // Generate properties
        var properties = type.GetMembers().OfType<IPropertySymbol>().ToList();
        for (var i = 0; i < properties.Count; i++)
        {
            var property = properties[i];
            
            // Property doc comment
            if (tsConfig.IncludeDocComments)
            {
                var propDoc = _docGenerator.GenerateDocComment(property, indent);
                if (!string.IsNullOrEmpty(propDoc))
                {
                    sb.AppendLine(propDoc);
                }
            }

            // Property declaration
            var readonlyModifier = tsConfig.UseReadonlyModifier && property.IsReadOnly ? "readonly " : "";
            var propertyName = NamingHelper.FormatName(property.Name, tsConfig.PropertyCase);
            var propertyType = _typeMapper.MapType(property.Type);
            var nullable = property.NullableAnnotation == NullableAnnotation.Annotated ? "?" : "";

            sb.AppendLine($"{indent}{readonlyModifier}{propertyName}{nullable}: {propertyType};");

            // Add newline between properties (except after last)
            if (tsConfig.AddNewlineBetweenProperties && i < properties.Count - 1)
            {
                sb.AppendLine();
            }
        }

        sb.AppendLine("}");

        // Generate file path
        var @namespace = type.ContainingNamespace?.ToDisplayString() ?? "";
        var modulePath = NamingHelper.NamespaceToModulePath(@namespace, tsConfig.ModuleCase);
        var fileName = type.Name;
        var relativePath = string.IsNullOrEmpty(modulePath) 
            ? $"src/{fileName}.ts" 
            : $"src/{modulePath}/{fileName}.ts";

        return new TypeScriptGeneratedFile(type.Name, sb.ToString(), relativePath, @namespace);
    }
}
