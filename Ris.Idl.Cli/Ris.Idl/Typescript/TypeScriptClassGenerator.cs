using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ris.Idl.Typescript;

public class TypeScriptClassGenerator
{
    private const string EMPTY = "";
    private const string EXPORT = "export ";

    private readonly TypeScriptTypes _types = new();
    private readonly TypeScriptDocCommentGenerator _docCommentGenerator = new();

    public string Convert(string source, Config? config = null)
    {
        config ??= new TypescriptConfig();

        var tree = CSharpSyntaxTree.ParseText(source);
        var root = tree.GetRoot();

        var compilation = CSharpCompilation.Create(
            "Generator",
            new[] { tree },
            new[]
            {
                MetadataReference.CreateFromFile(
                    typeof(object).Assembly.Location)
            });

        var semanticModel = compilation.GetSemanticModel(tree);

        var clsSyntax = root.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault();

        if (clsSyntax == null)
        {
            throw new Exception("No class found");
        }

        var cls = semanticModel.GetDeclaredSymbol(clsSyntax)
                  as INamedTypeSymbol;

        if (cls == null)
        {
            throw new Exception("Could not resolve class symbol");
        }

        var sb = new StringBuilder();

        string export = cls.DeclaredAccessibility == Accessibility.Public
            ? EXPORT
            : EMPTY;

        var classDoc = _docCommentGenerator.GetDocComment(cls);

        if (!string.IsNullOrEmpty(classDoc))
        {
            sb.Append(classDoc);
        }

        sb.AppendLine($"{export}interface {cls.Name} {{");
        sb.AppendLine();       
        
        var properties = cls.GetMembers().OfType<IPropertySymbol>();
        foreach (var property in properties)
        {
            if (property.DeclaredAccessibility != Accessibility.Public)
            {
                continue;
            }

            var docComment = _docCommentGenerator.GetDocComment(property);

            if (!string.IsNullOrEmpty(docComment))
            {
                sb.Append($"    {docComment}");
            }

            var name = property.Name;
            var type = _types.ConvertType(property.Type);

            sb.AppendLine($"    {name}: {type};");
            sb.AppendLine();          
        }


        sb.AppendLine("}");

        return sb.ToString();
    }
}