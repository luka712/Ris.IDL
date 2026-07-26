using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Ris.Idl.Core;
using Ris.Idl.Symbols;
using Ris.Idl.TypeScript.Mapper;
using Ris.Idl.Utilities;

namespace Ris.Idl.TypeScript;

/// <summary>
/// Generates TypeScript enums from C# enums.
/// </summary>
public class TypeScriptEnumsGenerator: ITypeGenerator
{
    private readonly TypeScriptMapper _typeMapper;
    private readonly TypeScriptDocCommentGenerator _docGenerator = new();
    
    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public TypeScriptEnumsGenerator(ILogger logger)
    {
        _typeMapper = new TypeScriptMapper(logger);
    }
    
    /// <inheritdoc />
    public GeneratedFile Generate(IIdlNamedSymbol type, GeneratorConfig? config = null)
    {
        var @enum = type as IdlEnumSymbol;

        if (@enum is null)
        {
            throw new ArgumentException($"Expected {nameof(IdlEnumSymbol)}, got {type.GetType().Name}", nameof(type));
        }
        
        var tsConfig = config as TypeScriptConfig ?? new TypeScriptConfig();
        
        var sb = new StringBuilder();
        var indent = tsConfig.Indentation;

        // Generate doc comment for interface
        if (tsConfig.IncludeDocComments && @enum.DocComment is not null)
        {
            var docComment = _docGenerator.GenerateDocComment(@enum.DocComment);
            if (!string.IsNullOrEmpty(docComment))
            {
                sb.Append(docComment);
            }
        }

        // Interface declaration
        var exportKeyword = tsConfig.ExportTypes ? "export " : "";
        sb.AppendLine($"{exportKeyword}enum {type.Name} {{");

        // Generate properties
        Generate(@enum, sb, indent, tsConfig);
        
        sb.AppendLine("}");

        // Generate file path
        var @namespace = type.Namespace;
        var modulePath = NamingHelper.NamespaceToModulePath(@namespace, tsConfig.ModuleCase);
        var fileName = type.Name;
        
        // Build relative path with optional source folder prefix
        var prefix = string.IsNullOrEmpty(tsConfig.SourceFolderPrefix) ? "" : $"{tsConfig.SourceFolderPrefix}/";
        var relativePath = string.IsNullOrEmpty(modulePath) 
            ? $"{prefix}{fileName}.ts" 
            : $"{prefix}{modulePath}/{fileName}.ts";

        return new GeneratedFile(type, sb.ToString(), relativePath);
    }

    public bool CanGenerate(IIdlNamedSymbol type)
    {
        return type is IdlEnumSymbol;
    }

    private void Generate(IdlEnumSymbol type, StringBuilder sb, string indent, TypeScriptConfig tsConfig)
    {
        var fields = type.FieldSymbols;

        if (fields is null)
        {
            return;
        }

        for (var i = 0; i < fields.Count; i++)
        {
            var field = fields[i];
            var isLast = i == fields.Count - 1;
            
            // Property doc comment
            if (tsConfig.IncludeDocComments && field.DocComment is not null) 
            {
                var propDoc = _docGenerator.GenerateDocComment(field.DocComment);
                if (!string.IsNullOrEmpty(propDoc))
                {
                    sb.Append(propDoc);
                }
            }

            // Property declaration
            var enumValueName = NamingHelper.FormatName(field.Name, tsConfig.EnumKeyCase);
            var enumValue = field.Value;
            
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