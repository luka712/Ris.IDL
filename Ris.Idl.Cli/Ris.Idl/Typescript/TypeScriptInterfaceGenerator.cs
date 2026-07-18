using System.Text;
using Microsoft.CodeAnalysis;
using Ris.Idl.Interfaces;

namespace Ris.Idl.Typescript;

public class TypeScriptInterfaceGenerator : BaseTypeScriptGenerator, IInterfaceGenerator
{
    private const string EMPTY = "";
    private const string EXPORT = "export ";

    private readonly TypeScriptTypes _types = new();
    private readonly TypeScriptDocCommentGenerator _docCommentGenerator = new();

    // public IInterface Convert(string source, Config? config = null)
    // {
    //     config ??= new TypescriptConfig();
    //
    //     var tree = CSharpSyntaxTree.ParseText(source);
    //     var root = tree.GetRoot();
    //
    //     var compilation = CSharpCompilation.Create(
    //         "Generator",
    //         new[] { tree },
    //         new[]
    //         {
    //             MetadataReference.CreateFromFile(
    //                 typeof(object).Assembly.Location)
    //         });
    //
    //     var semanticModel = compilation.GetSemanticModel(tree);
    //
    //     var interfaceSyntax = root.DescendantNodes()
    //         .OfType<InterfaceDeclarationSyntax>()
    //         .FirstOrDefault();
    //
    //     if (interfaceSyntax == null)
    //     {
    //         throw new Exception("No interface found");
    //     }
    //
    //     var iface = semanticModel.GetDeclaredSymbol(interfaceSyntax)
    //                 as INamedTypeSymbol;
    //
    //     if (iface == null)
    //     {
    //         throw new Exception("Could not resolve interface symbol");
    //     }
    //
    //     var sb = new StringBuilder();
    //
    //     string export = iface.DeclaredAccessibility == Accessibility.Public
    //         ? EXPORT
    //         : EMPTY;
    //
    //
    //     var interfaceDoc = _docCommentGenerator.GetDocComment(iface);
    //
    //     if (!string.IsNullOrEmpty(interfaceDoc))
    //     {
    //         sb.AppendLine(interfaceDoc);
    //     }
    //
    //
    //     sb.AppendLine($"{export}interface {iface.Name} {{");
    //     sb.AppendLine();
    //
    //
    //     foreach (var property in iface.GetMembers()
    //                  .OfType<IPropertySymbol>())
    //     {
    //         var docComment = _docCommentGenerator.GetDocComment(property);
    //
    //         if (!string.IsNullOrEmpty(docComment))
    //         {
    //             foreach (var line in docComment.Split('\n'))
    //             {
    //                 sb.AppendLine($"    {line}");
    //             }
    //         }
    //
    //         var @readonly = property.IsReadOnly ? "readonly " : EMPTY;
    //         var name = FormatName(property.Name, config.PropertyCase);
    //         var type = _types.ConvertType(property.Type);
    //
    //         sb.AppendLine($"    {@readonly}{name}: {type};");
    //         sb.AppendLine();
    //     }
    //
    //
    //     sb.AppendLine("}");
    //
    //     return sb.ToString();
    // }
    

    public IInterface Convert(INamedTypeSymbol iface, Config? config = null)
    {
       config ??= new TypescriptConfig();
       
        if (iface == null)
        {
            throw new Exception("Could not resolve interface symbol");
        }

        var sourceCodeBuilder = new StringBuilder();

        string export = iface.DeclaredAccessibility == Accessibility.Public
            ? EXPORT
            : EMPTY;

        var interfaceDoc = _docCommentGenerator.GetDocComment(iface);
        var @namespace = iface.ContainingNamespace.ToDisplayString();
        var module = FormatModule(@namespace, config.ModuleCase);

        if (!string.IsNullOrEmpty(interfaceDoc))
        {
            sourceCodeBuilder.AppendLine(interfaceDoc);
        }

        sourceCodeBuilder.AppendLine($"{export}interface {iface.Name} {{");
        sourceCodeBuilder.AppendLine();


        foreach (var property in iface.GetMembers().OfType<IPropertySymbol>())
        {
            var docComment = _docCommentGenerator.GetDocComment(property);

            if (!string.IsNullOrEmpty(docComment))
            {
                foreach (var line in docComment.Split('\n'))
                {
                    sourceCodeBuilder.AppendLine($"    {line}");
                }
            }

            var @readonly = property.IsReadOnly ? "readonly " : EMPTY;
            var name = FormatName(property.Name, config.PropertyCase);
            var type = _types.ConvertType(property.Type);

            sourceCodeBuilder.AppendLine($"    {@readonly}{name}: {type};");
            sourceCodeBuilder.AppendLine();
        }


        sourceCodeBuilder.AppendLine("}");
        
        var fileName = FormatFileName(iface.Name);

        return new TypeScriptInterface()
        {
            SourceCode = sourceCodeBuilder.ToString(),
            Name = iface.Name,
            FilePath = $"src/{@module}/{fileName}.ts" 
        };
    }
}