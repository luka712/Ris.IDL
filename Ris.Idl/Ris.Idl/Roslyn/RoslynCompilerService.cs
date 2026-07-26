using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace Ris.Idl.Roslyn;

/// <summary>
/// The roslyn compiler service.
/// </summary>
internal class RoslynCompilerService
{
    private static bool _msBuildRegistered;
    private static readonly object _lock = new();
    
    /// <summary>
    /// Read the symbols from a roslyn project.
    /// </summary>
    /// <param name="projectPath">The file path to the project.</param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException">TODO</exception>
    /// <exception cref="InvalidOperationException">TODO</exception>
    internal async Task<IReadOnlyList<INamedTypeSymbol>> ReadRoslynSymbolsAsync(string projectPath)
    {
        if (!File.Exists(projectPath))
        {
            throw new FileNotFoundException("Project file not found", projectPath);
        }

        EnsureMsBuildRegistered();

        using var workspace = MSBuildWorkspace.Create();

        workspace.RegisterWorkspaceFailedHandler(e =>
        {
            if (e.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure)
            {
                Console.Error.WriteLine($"Workspace error: {e.Diagnostic.Message}");
            }
        });

        var project = await workspace.OpenProjectAsync(projectPath);
        var compilation = await project.GetCompilationAsync();

        if (compilation is null)
        {
            throw new InvalidOperationException("Failed to compile project");
        }

        return GetTypesFromNamespace(compilation.Assembly.GlobalNamespace).ToList();
    }
    
    /// <summary>
    /// Ensures MSBuild is registered (only once per process).
    /// </summary>
    private static void EnsureMsBuildRegistered()
    {
        lock (_lock)
        {
            if (_msBuildRegistered) return;
        }

        lock (_lock)
        {
            if (_msBuildRegistered) return;

            MSBuildLocator.RegisterDefaults();
            _msBuildRegistered = true;
        }
    }
    
    /// <summary>
    /// Recursively gets all types from a namespace.
    /// </summary>
    private static IEnumerable<INamedTypeSymbol> GetTypesFromNamespace(INamespaceSymbol ns)
    {
        foreach (var type in ns.GetTypeMembers())
        {
            yield return type;

            // Get nested types
            foreach (var nested in GetNestedTypes(type))
            {
                yield return nested;
            }
        }

        foreach (var childNs in ns.GetNamespaceMembers())
        {
            foreach (var type in GetTypesFromNamespace(childNs))
            {
                yield return type;
            }
        }
    }

    /// <summary>
    /// Gets nested types from a type.
    /// </summary>
    private static IEnumerable<INamedTypeSymbol> GetNestedTypes(INamedTypeSymbol type)
    {
        foreach (var nested in type.GetTypeMembers())
        {
            yield return nested;

            foreach (var deepNested in GetNestedTypes(nested))
            {
                yield return deepNested;
            }
        }
    }
}