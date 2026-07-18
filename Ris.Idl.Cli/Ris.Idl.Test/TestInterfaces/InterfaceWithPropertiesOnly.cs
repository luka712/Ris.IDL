namespace Ris.Idl.Test.TestInterfaces;

/// <summary>
/// This is an interface with properties only.
/// </summary>
public interface IInterfaceWithPropertiesOnly
{
    /// <summary>
    /// This is a string property.
    /// </summary>
    public string? StringProperty { get; }
    
    /// <summary>
    /// This is an int property.
    /// </summary>
    public int IntProperty { get; }
    
    /// <summary>
    /// This is a bool property.
    /// </summary>
    public bool BoolProperty { get; }
}