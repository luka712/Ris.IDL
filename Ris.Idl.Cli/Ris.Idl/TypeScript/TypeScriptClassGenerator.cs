using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Ris.Idl.Core;
using Ris.Idl.Symbols;

namespace Ris.Idl.TypeScript;

/// <summary>
/// Generates TypeScript interfaces/classes from C# classes.
/// </summary>
public class TypeScriptClassGenerator : ITypeGenerator
{
    private readonly TypeScriptTypeMapper _typeMapper;
    private readonly TypeScriptDocCommentGenerator _docGenerator = new();

    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public TypeScriptClassGenerator(ILogger logger)
    {
        _typeMapper = new TypeScriptTypeMapper(logger);
    }
    
    /// <inheritdoc />
    public bool CanGenerate(INamedTypeSymbol type)
    {
        return type.TypeKind == TypeKind.Class && !type.IsStatic;
    }

    /// <inheritdoc />
    public IGeneratedFile Generate(INamedTypeSymbol type, GeneratorConfig? config = null)
    {
        var tsConfig = config as TypeScriptConfig ?? new TypeScriptConfig();
        
        if (type.TypeKind != TypeKind.Class)
        {
            throw new ArgumentException($"Expected class, got {type.TypeKind}", nameof(type));
        }

        var sb = new StringBuilder();
        var indent = tsConfig.Indentation;

        // Generate doc comment for class
        if (tsConfig.IncludeDocComments)
        {
            var docComment = _docGenerator.GenerateDocComment(type);
            if (!string.IsNullOrEmpty(docComment))
            {
                sb.AppendLine(docComment);
            }
        }

        // Type declaration (interface or class based on config)
        var exportKeyword = tsConfig.ExportTypes ? "export " : "";
        var typeKeyword = tsConfig.UseInterfaceForClasses ? "interface" : "class";
        sb.AppendLine($"{exportKeyword}{typeKeyword} {type.Name} {{");

        // Generate public properties only
        var properties = type.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.DeclaredAccessibility == Accessibility.Public)
            .ToList();

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
            var propertyName = NamingHelper.FormatName(property.Name, tsConfig.PropertyCase);
            var propertyType = _typeMapper.MapType(property.Type);
            var nullable = property.NullableAnnotation == NullableAnnotation.Annotated ? "?" : "";

            sb.AppendLine($"{indent}{propertyName}{nullable}: {propertyType};");

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
        var fileName = NamingHelper.ToCamelCase(type.Name);
        
        // Build relative path with optional source folder prefix
        var prefix = string.IsNullOrEmpty(tsConfig.SourceFolderPrefix) ? "" : $"{tsConfig.SourceFolderPrefix}/";
        var relativePath = string.IsNullOrEmpty(modulePath) 
            ? $"{prefix}{fileName}.ts" 
            : $"{prefix}{modulePath}/{fileName}.ts";

        return new TypeScriptGeneratedFile(type.Name, sb.ToString(), relativePath, @namespace);
    }
}
