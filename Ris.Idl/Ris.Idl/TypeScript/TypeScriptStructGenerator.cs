using System.Text;
using Microsoft.Extensions.Logging;
using Ris.Idl.Core;
using Ris.Idl.Symbols;
using Ris.Idl.TypeScript.Mapper;
using Ris.Idl.Utilities;

namespace Ris.Idl.TypeScript;

/// <summary>
/// Generates TypeScript class from C# structs.
/// </summary>
public class TypeScriptStructGenerator : ITypeGenerator
{
    private readonly ILogger _logger;
    private readonly TypeScriptMapper _typeMapper;
    private readonly TypeScriptDocCommentGenerator _docGenerator = new();

    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public TypeScriptStructGenerator(ILogger logger)
    {
        _logger = logger;
        _typeMapper = new TypeScriptMapper(logger);
    }
    
    /// <inheritdoc />
    public GeneratedFile Generate(IIdlNamedSymbol type, GeneratorConfig? config = null)
    {
        var @class = type as IdlStructSymbol;

        if (@class is null)
        {
            throw new ArgumentException($"Expected {nameof(IdlStructSymbol)}, got {type.GetType().Name}", nameof(type));
        }
        
        var tsConfig = config as TypeScriptConfig ?? new TypeScriptConfig();
        
        var sb = new StringBuilder();
        var indent = tsConfig.Indentation;

        // Generate doc comment for interface
        if (tsConfig.IncludeDocComments && @class.DocComment is not null)
        {
            var docComment = _docGenerator.GenerateDocComment(@class.DocComment);
            if (!string.IsNullOrEmpty(docComment))
            {
                sb.AppendLine(docComment);
            }
        }

        // Interface declaration
        var inheritedMembers = @class.Inherits?.Select(x => x.Name).ToList();
        var interfacesStr = inheritedMembers != null ? String.Join(',', inheritedMembers) : String.Empty;

        var exportKeyword = tsConfig.ExportTypes && @class.Visibility == IdlVisibility.PUBLIC ? "export " : "";
        var extendsKeyword = !string.IsNullOrEmpty(interfacesStr) ? $" extends {interfacesStr}" : "";
        sb.AppendLine($"{exportKeyword}class {type.Name}{extendsKeyword} {{");
        sb.AppendLine();

        // Generate properties, events and methods
        GenerateProperties(@class, sb, indent, tsConfig); 
        GenerateEvents(@class, sb, indent, tsConfig);
        GenerateMethods(@class, sb, indent, tsConfig);

        sb.AppendLine("}");

        // Generate file path
        var modulePath = NamingHelper.NamespaceToModulePath(@class.Namespace, tsConfig.ModuleCase);
        var fileName = type.Name;

        // Build relative path with optional source folder prefix
        var prefix = string.IsNullOrEmpty(tsConfig.SourceFolderPrefix) ? "" : $"{tsConfig.SourceFolderPrefix}/";
        var relativePath = string.IsNullOrEmpty(modulePath)
            ? $"{prefix}{fileName}.ts"
            : $"{prefix}{modulePath}/{fileName}.ts";

        return new GeneratedFile(type, sb.ToString(), relativePath);
    }

    /// <inheritdoc />
    public bool CanGenerate(IIdlNamedSymbol type)
    {
        return type is IdlStructSymbol;
    }
    
    private void GenerateProperties(IdlStructSymbol type, StringBuilder sb, string indent, TypeScriptConfig tsConfig)
    {
        var properties = type.PropertySymbols;
        if (properties is null)
        {
            return;
        }

        for (var i = 0; i < properties.Count; i++)
        {
            var property = properties[i];
            var docComment = string.Empty;

            // Property doc comment
            if (tsConfig.IncludeDocComments && property.DocComment is not null)
            {
                docComment = _docGenerator.GenerateDocComment(property.DocComment);
            }

            // Property declaration
            var hasSetter = !property.IsReadonly;
            var propertyName = NamingHelper.FormatName(property.Name, tsConfig.PropertyCase);
            var propertyType = _typeMapper.MapType(property.TypeSymbol);
            var nullable = property.NullableAnnotation ? "?" : "";
            var @static = property.IsStatic ? "static " : "";
            var visibility = _typeMapper.MapVisibility(property.Visibility);
            var defaultValue = _typeMapper.GetDefaultValue(property.TypeSymbol);

            // Getter
            if (!String.IsNullOrEmpty(docComment))
            {
                sb.AppendLine(docComment);
            }

            sb.AppendLine($"{indent}{visibility} {@static}get {propertyName}{nullable}(): {propertyType} {{");
            sb.AppendLine($"{indent}    return {defaultValue};");
            sb.AppendLine("}}");

            // Setter
            if (hasSetter)
            {
                if (!String.IsNullOrEmpty(docComment))
                {
                    sb.AppendLine(docComment);
                }

                sb.AppendLine($"{indent}{visibility} {@static}set {propertyName}{nullable}(value: {propertyType}): void {{");
                sb.AppendLine("");
                sb.AppendLine("}");
            }


            // Add newline between properties (except after last)
            if (tsConfig.AddNewlineBetweenProperties && i < properties.Count - 1)
            {
                sb.AppendLine();
            }
        }
    }
    
    private void GenerateEvents(IdlStructSymbol type, StringBuilder sb, string indent, TypeScriptConfig tsConfig)
    {
        // Generate events
        var events = type.EventSymbols;

        if (events is null)
        {
            return;
        }
        
        for (var i = 0; i < events.Count; i++)
        {
            var @event = events[i];

            var docStrBuilder = new StringBuilder();

            // Property doc comment
            if (tsConfig.IncludeDocComments && @event.DocComment is not null)
            {
                var propDoc = _docGenerator.GenerateDocComment(@event.DocComment);
                if (!string.IsNullOrEmpty(propDoc))
                {
                    docStrBuilder.Append(propDoc);
                }
            }

            // Property declaration
            StringBuilder parametersBuilder = new();

            if (@event.Parameters != null)
            {
                for (int j = 0; j < @event.Parameters.Count; j++)
                {
                    var param = @event.Parameters[j];
                    parametersBuilder.Append(param.Name);
                    parametersBuilder.Append(": ");
                    parametersBuilder.Append(_typeMapper.MapType(param.Type));

                    if (j < @event.Parameters.Count - 1)
                    {
                        parametersBuilder.Append(", ");
                    }
                }
            }

            var addEventName = NamingHelper.FormatName($"Add{@event.Name}Listener", tsConfig.PropertyCase);
            var removeEventName = NamingHelper.FormatName($"Remove{@event.Name}Listener", tsConfig.PropertyCase);

            sb.Append(docStrBuilder.ToString());
            sb.Append($"{indent}{addEventName}({parametersBuilder}): void;");

            if (tsConfig.AddNewlineBetweenMethods)
            {
                sb.AppendLine();
            }

            sb.Append(docStrBuilder.ToString());
            sb.Append($"{indent}{removeEventName}(event: ({parametersBuilder}) => void): void;");

            // Add newline between methods (except after last)
            if (tsConfig.AddNewlineBetweenMethods && i < events.Count - 1)
            {
                sb.AppendLine();
            }
        }
    }

    private void GenerateMethods(IdlStructSymbol type, StringBuilder sb, string indent, TypeScriptConfig tsConfig)
    {
        if (type.MethodSymbols is null)
        {
            return;
        }
        
        // Generate methods
        var methods = type.MethodSymbols;

        for (var i = 0; i < methods.Count; i++)
        {
            var method = methods[i];

            // Property doc comment
            if (tsConfig.IncludeDocComments && method.DocComment is not null)
            {
                var propDoc = _docGenerator.GenerateDocComment(method.DocComment);
                if (!String.IsNullOrEmpty(propDoc))
                {
                    sb.Append(propDoc);
                }
            }

            // Property declaration
            var methodName = NamingHelper.FormatName(method.Name, tsConfig.PropertyCase);
            var returnType = _typeMapper.MapType(method.ReturnType);
            var nullable = method.NullableAnnotation ? "?" : "";
            var visibility = _typeMapper.MapVisibility(method.Visibility);
            var defaultValue = _typeMapper.GetDefaultValue(method.ReturnType);

            StringBuilder parametersBuilder = new();
            if (method.Parameters is not null)
            {
                for (int j = 0; j < method.Parameters.Count; j++)
                {
                    var param = method.Parameters[j];

                    // If it's null annotated or has a default value, add a ? to the type.
                    var nullableParam = param.NullableAnnotation ? "?" : String.Empty;

                    parametersBuilder.Append(param.Name);
                    parametersBuilder.Append($"{nullableParam}: ");
                    parametersBuilder.Append(_typeMapper.MapType(param.Type));

                    if (j < method.Parameters.Count - 1)
                    {
                        parametersBuilder.Append(", ");
                    }
                }
            }

            sb.AppendLine($"{indent}{visibility} {methodName}({parametersBuilder}): {returnType}{nullable} {{");
            if (!String.IsNullOrEmpty(defaultValue))
            {
                sb.AppendLine($"{indent}    return {defaultValue};");
            }
            sb.AppendLine("}}");

            // Add newline between methods (except after last)
            if (tsConfig.AddNewlineBetweenMethods && i < methods.Count - 1)
            {
                sb.AppendLine();
            }
        }
    }
}