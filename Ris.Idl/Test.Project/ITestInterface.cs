namespace Test.Project;

/// <summary>
/// The test interface.
/// </summary>
public interface ITestInterface
{
    /// <summary>
    /// Test event.
    /// </summary>
    event EventHandler<EventArgs>? TestEvent;
    
    /// <summary>
    /// Test property 1.
    /// </summary>
    public bool TestProperty1 { get; }
    
    /// <summary>
    /// Test property 2.
    /// </summary>
    public int TestProperty2 { get; }
}