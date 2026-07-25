namespace Ris.Idl.Test;

[TestFixture]
public class IdlProjectTests
{
    private ProjectLoader _loader = null!;

    [SetUp]
    public void Setup()
    {
        _loader = new ProjectLoader();
    }
    
    [Test]
    public async Task ReadProject_ReadsProjectFile()
    {
        // Arrange
        var projectPath = "../../../../Test.Project/Test.Project.csproj";
        
        // Act
        var project = await _loader.ReadProjectAsync(projectPath);
        
        // Assert
        Assert.That(project, Is.Not.Null);
        
        var testInterface = project.Interfaces?.FirstOrDefault(f => f.Name == "ITestInterface");
        Assert.That(testInterface, Is.Not.Null);
    }
}