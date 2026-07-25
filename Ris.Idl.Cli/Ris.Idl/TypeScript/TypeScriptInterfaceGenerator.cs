using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Ris.Idl.Core;
using Ris.Idl.Symbols;

namespace Ris.Idl.TypeScript;

/// <summary>
/// Generates TypeScript interfaces from C# interfaces.
/// </summary>
public class TypeScriptInterfaceGenerator : ITypeGenerator
{
    private readonly ILogger _logger;
    private readonly TypeScriptTypeMapper _typeMapper;
    private readonly TypeScriptDocCommentGenerator _docGenerator = new();
    
    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public TypeScriptInterfaceGenerator(ILogger logger)
    {
        _logger = logger;
        _typeMapper = new TypeScriptTypeMapper(logger);
    }

    /// <inheritdoc />
    public bool CanGenerate(INamedTypeSymbol type)
    {
        return type.TypeKind == TypeKind.Interface;
    }

    /// <inheritdoc />
    public IGeneratedFile Generate(INamedTypeSymbol type, GeneratorConfig? config = null)
    {
        _logger.LogDebug("Generating TypeScript interface for {Type}", type.Name);
        
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
        var interfaces = type.Interfaces.Select(i => i.ToDisplayString());
        var interfacesStr = String.Join(',', interfaces);
        
        var exportKeyword = tsConfig.ExportTypes ? "export " : "";
        var extendsKeyword = !string.IsNullOrEmpty(interfacesStr) ? $" extends {interfacesStr}" : "";
        sb.AppendLine($"{exportKeyword}interface {type.Name}{extendsKeyword} {{");

        // Generate properties, events and methods
        GenerateProperties(type, sb, indent, tsConfig);
        GenerateEvents(type, sb, indent, tsConfig);
        GenerateMethods(type, sb, indent, tsConfig);
        
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

    private void GenerateProperties(INamedTypeSymbol type, StringBuilder sb, string indent, TypeScriptConfig tsConfig)
    {
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
    }
    
    private void GenerateEvents(INamedTypeSymbol type, StringBuilder sb, string indent, TypeScriptConfig tsConfig)
    {
        // Generate events
        var events = type.GetMembers()
            .OfType<IEventSymbol>()
            .Where(x => x.Kind == SymbolKind.Event)
            .ToList();
        
        for (var i = 0; i < events.Count; i++)
        {
            var @event = events[i];
            
            var docStrBuilder = new StringBuilder();
            
            // Property doc comment
            if (tsConfig.IncludeDocComments)
            {
                var propDoc = _docGenerator.GenerateDocComment(@event, indent);
                if (!string.IsNullOrEmpty(propDoc))
                {
                    docStrBuilder.AppendLine(propDoc);
                }
            }

            // Property declaration
            var eventName = @event.Name;

            StringBuilder parametersBuilder = new();

            if (@event.RaiseMethod != null)
            {
                var parameters = @event.RaiseMethod.Parameters;
                for (int j = 0; j < parameters.Length; j++)
                {
                    var param = parameters[j];
                    parametersBuilder.Append(param.Name);
                    parametersBuilder.Append(": ");
                    parametersBuilder.Append(_typeMapper.MapType(param.Type));

                    if (j < parameters.Length - 1)
                    {
                        parametersBuilder.Append(", ");
                    }
                }
            }

            var addEventName =  NamingHelper.FormatName($"Add{@event.Name}Listener", tsConfig.PropertyCase);
            var removeEventName = NamingHelper.FormatName($"Remove{@event.Name}Listener", tsConfig.PropertyCase);

            sb.Append(docStrBuilder.ToString());
            sb.Append($"{indent}{addEventName}(event: ({parametersBuilder}) => void): void;");

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
    
    private void GenerateMethods(INamedTypeSymbol type, StringBuilder sb, string indent, TypeScriptConfig tsConfig)
    {
        // Generate methods
        var methods = type.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(x => x.MethodKind == MethodKind.Ordinary)
            .ToList();
        
        for (var i = 0; i < methods.Count; i++)
        {
            var method = methods[i];
            
            // Property doc comment
            if (tsConfig.IncludeDocComments)
            {
                var propDoc = _docGenerator.GenerateDocComment(method, indent);
                if (!string.IsNullOrEmpty(propDoc))
                {
                    sb.AppendLine(propDoc);
                }
            }

            // Property declaration
            var methodName = NamingHelper.FormatName(method.Name, tsConfig.PropertyCase);
            var returnType = _typeMapper.MapType(method.ReturnType);
            var nullable = method.ReturnType.NullableAnnotation == NullableAnnotation.Annotated ? "?" : "";

            StringBuilder parametersBuilder = new();
            for(int j = 0; j < method.Parameters.Length; j++)
            {
                var param = method.Parameters[j];
                
                // If it's null annotated or has a default value, add a ? to the type.
                var nullableParam = param.Type.NullableAnnotation == NullableAnnotation.Annotated || param.HasExplicitDefaultValue ? "?" : "";
                
                parametersBuilder.Append(param.Name);
                parametersBuilder.Append($"{nullableParam}: ");
                parametersBuilder.Append(_typeMapper.MapType(param.Type));

                if (j < method.Parameters.Length - 1)
                {
                    parametersBuilder.Append(", ");
                }
            }
            
            sb.AppendLine($"{indent}{methodName}({parametersBuilder}): {returnType}{nullable};");

            // Add newline between methods (except after last)
            if (tsConfig.AddNewlineBetweenMethods && i < methods.Count - 1)
            {
                sb.AppendLine();
            }
        }
    }
}
