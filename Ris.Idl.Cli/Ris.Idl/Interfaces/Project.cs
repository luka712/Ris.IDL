using Ris.Idl.Interfaces;

namespace Ris.Idl;

public class Project
{
    public Project(IReadOnlyList<IInterface> interfaces)
    {
        Interfaces = interfaces;
    }
    
    public IReadOnlyList<IInterface> Interfaces { get; }
}