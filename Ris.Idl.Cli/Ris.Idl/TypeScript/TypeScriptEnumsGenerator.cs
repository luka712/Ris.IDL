using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Ris.Idl.Core;

namespace Ris.Idl.TypeScript;

/// <summary>
/// Generates TypeScript enums from C# enums.
/// </summary>
public class TypeScriptEnumsGenerator: ITypeGenerator
{
    private readonly TypeScriptTypeMapper _typeMapper;
    private readonly TypeScriptDocCommentGenerator _docGenerator = new();
    
    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public TypeScriptEnumsGenerator(ILogger logger)
    {
        _typeMapper = new TypeScriptTypeMapper(logger);
    }

    /// <inheritdoc />
    public bool CanGenerate(INamedTypeSymbol type)
    {
        return type.TypeKind == TypeKind.Enum;
    }

    /// <inheritdoc />
    public IGeneratedFile Generate(INamedTypeSymbol type, GeneratorConfig? config = null)
    {
        var tsConfig = config as TypeScriptConfig ?? new TypeScriptConfig();
        
        if (type.TypeKind != TypeKind.Enum)
        {
            throw new ArgumentException($"Expected enum, got {type.TypeKind}", nameof(type));
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
        sb.AppendLine($"{exportKeyword}enum {type.Name} {{");

        // Generate properties
        Generate(type, sb, indent, tsConfig);
        
        sb.AppendLine("}");

        // Generate file path
        var @namespace = type.ContainingNamespace?.ToDisplayString() ?? "";
        var modulePath = NamingHelper.NamespaceToModulePath(@namespace, tsConfig.ModuleCase);
        var fileName = type.Name;
        
        // Build relative path with optional source folder prefix
        var prefix = string.IsNullOrEmpty(tsConfig.SourceFolderPrefix) ? "" : $"{tsConfig.SourceFolderPrefix}/";
        var relativePath = string.IsNullOrEmpty(modulePath) 
            ? $"{prefix}{fileName}.ts" 
            : $"{prefix}{modulePath}/{fileName}.ts";

        return new TypeScriptGeneratedFile(type.Name, sb.ToString(), relativePath, @namespace);
    }

    private void Generate(INamedTypeSymbol type, StringBuilder sb, string indent, TypeScriptConfig tsConfig)
    {
        var members = type.GetMembers()
            .OfType<IFieldSymbol>()
            .ToList();

        for (var i = 0; i < members.Count; i++)
        {
            var @enum = members[i];
            var isLast = i == members.Count - 1;
            
            // Property doc comment
            if (tsConfig.IncludeDocComments)
            {
                var propDoc = _docGenerator.GenerateDocComment(@enum, indent);
                if (!string.IsNullOrEmpty(propDoc))
                {
                    sb.AppendLine(propDoc);
                }
            }

            // Property declaration
            var enumValueName = NamingHelper.FormatName(@enum.Name, tsConfig.EnumKeyCase);
            var enumValue = @enum.HasConstantValue ? @enum.ConstantValue : null;
            
            sb.Append($"{indent}{enumValueName}");
            if (enumValue != null)
            {
                sb.Append($" = {enumValue}");
                if (!isLast)
                {
                    sb.Append(",");
                }
            }

            sb.AppendLine();
            
            // Add newline between properties (except after last)
            if (tsConfig.AddNewLinesBetweenEnumValues && !isLast)
            {
                sb.AppendLine();
            }
        }
    }
}