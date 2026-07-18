using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Ris.Idl.Interfaces;
using Ris.Idl.Typescript;

namespace Ris.Idl;

public class Loader
{
    private readonly IInterfaceGenerator _interfaceGenerator = new TypeScriptInterfaceGenerator();
    
    public async Task<Project> LoadProjectAsync(string path)
    {
        // Check if the file exists
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("File not found", path);
        }

        MSBuildLocator.RegisterDefaults();
        
        using (var workspace = MSBuildWorkspace.Create())
        {
            workspace.WorkspaceFailed += (_, e) =>
            {
                Console.WriteLine(
                    $"Workspace error: {e.Diagnostic.Message}");
            };
            
            var project = await workspace.OpenProjectAsync(path);
            var compilation = await project.GetCompilationAsync();

            if (compilation is null)
            {
                throw new InvalidOperationException("Compilation is null");
            }
            
            foreach (var diagnostic in compilation.GetDiagnostics())
            {
                Console.WriteLine(diagnostic.ToString());
            }
            
            var interfaces = GetInterfaces(compilation);
            var projectInterfaces = new List<IInterface>();
            
            foreach (var iface in interfaces)
            {
                var @interface = _interfaceGenerator.Convert(iface, new TypescriptConfig());
                projectInterfaces.Add(@interface);
            }
            
            Console.WriteLine();
            
            return new Project( projectInterfaces);
        }
    }
    
    static IEnumerable<INamedTypeSymbol> GetInterfaces(Compilation compilation)
    {
        return GetInterfaces(compilation.Assembly.GlobalNamespace);
    }

    static IEnumerable<INamedTypeSymbol> GetInterfaces(INamespaceSymbol ns)
    {
        foreach (var type in ns.GetTypeMembers())
        {
            foreach (var iface in GetInterfaces(type))
                yield return iface;
        }

        foreach (var childNs in ns.GetNamespaceMembers())
        {
            foreach (var iface in GetInterfaces(childNs))
                yield return iface;
        }
    }

    static IEnumerable<INamedTypeSymbol> GetInterfaces(INamedTypeSymbol type)
    {
        if (type.TypeKind == TypeKind.Interface)
            yield return type;

        foreach (var nested in type.GetTypeMembers())
        {
            foreach (var iface in GetInterfaces(nested))
                yield return iface;
        }
    }

    public async Task LoadSolutionAsync(string path)
    {
        using (var workspace = MSBuildWorkspace.Create())
        {
            var solution = await workspace.OpenSolutionAsync(path);

            foreach (var project in solution.Projects)
            {
                var compilation = await project.GetCompilationAsync();

                // Perform analysis...
            }
        }
    }
}