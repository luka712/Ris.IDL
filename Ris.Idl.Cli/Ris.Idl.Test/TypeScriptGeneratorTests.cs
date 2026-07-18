using Ris.Idl;
using Ris.Idl.Core;
using Ris.Idl.TypeScript;

namespace Ris.Idl.Test;

[TestFixture]
public class TypeScriptGeneratorTests
{
    private ProjectLoader _loader = null!;

    [SetUp]
    public void Setup()
    {
        _loader = new ProjectLoader();
    }

    [Test]
    public async Task LoadProject_GeneratesInterfaceFiles()
    {
        // Arrange
        var projectPath = "../../../../Test.Project/Test.Project.csproj";
        
        // Act
        var files = await _loader.LoadProjectAsync(projectPath);
        
        // Assert
        Assert.That(files, Is.Not.Empty);
        
        var testInterface = files.FirstOrDefault(f => f.Name == "ITestInterface");
        Assert.That(testInterface, Is.Not.Null);
        Assert.That(testInterface!.Content, Does.Contain("interface ITestInterface"));
        Assert.That(testInterface.Content, Does.Contain("export"));
    }

    [Test]
    public async Task LoadProject_GeneratesPropertiesWithCorrectTypes()
    {
        // Arrange
        var projectPath = "../../../../Test.Project/Test.Project.csproj";
        
        // Act
        var files = await _loader.LoadProjectAsync(projectPath);
        var testInterface = files.FirstOrDefault(f => f.Name == "ITestInterface");
        
        // Assert
        Assert.That(testInterface, Is.Not.Null);
        Assert.That(testInterface!.Content, Does.Contain("boolean")); // bool -> boolean
        Assert.That(testInterface.Content, Does.Contain("number")); // int -> number
    }

    [Test]
    public async Task LoadProject_GeneratesDocComments()
    {
        // Arrange
        var projectPath = "../../../../Test.Project/Test.Project.csproj";
        
        // Act
        var files = await _loader.LoadProjectAsync(projectPath);
        var testInterface = files.FirstOrDefault(f => f.Name == "ITestInterface");
        
        // Assert
        Assert.That(testInterface, Is.Not.Null);
        Assert.That(testInterface!.Content, Does.Contain("/**")); // JSDoc comment
        Assert.That(testInterface.Content, Does.Contain("*/")); 
    }

    [Test]
    public async Task LoadProject_UsesCamelCaseForProperties()
    {
        // Arrange
        var projectPath = "../../../../Test.Project/Test.Project.csproj";
        var config = new TypeScriptConfig { PropertyCase = NamingCase.Camel };
        
        // Act
        var files = await _loader.LoadProjectAsync(projectPath, config);
        var testInterface = files.FirstOrDefault(f => f.Name == "ITestInterface");
        
        // Assert
        Assert.That(testInterface, Is.Not.Null);
        Assert.That(testInterface!.Content, Does.Contain("testProperty1")); // camelCase
    }

    [Test]
    public async Task GenerateProject_CreatesPackageJson()
    {
        // Arrange
        var projectPath = "../../../../Test.Project/Test.Project.csproj";
        var config = new TypeScriptProjectConfiguration
        {
            Name = "test-types",
            Version = "1.0.0",
            Description = "Test generated types"
        };
        
        // Act
        var project = await _loader.GenerateProjectAsync(projectPath, config);
        
        // Assert
        Assert.That(project.ProjectFiles, Does.ContainKey("package.json"));
        Assert.That(project.ProjectFiles["package.json"], Does.Contain("\"name\": \"test-types\""));
        Assert.That(project.ProjectFiles["package.json"], Does.Contain("\"version\": \"1.0.0\""));
    }

    [Test]
    public async Task GenerateProject_CreatesTsConfig()
    {
        // Arrange
        var projectPath = "../../../../Test.Project/Test.Project.csproj";
        var config = new TypeScriptProjectConfiguration
        {
            TypeScriptTarget = "ES2022",
            StrictMode = true
        };
        
        // Act
        var project = await _loader.GenerateProjectAsync(projectPath, config);
        
        // Assert
        Assert.That(project.ProjectFiles, Does.ContainKey("tsconfig.json"));
        Assert.That(project.ProjectFiles["tsconfig.json"], Does.Contain("\"target\": \"ES2022\""));
        Assert.That(project.ProjectFiles["tsconfig.json"], Does.Contain("\"strict\": true"));
    }

    [Test]
    public async Task GenerateProject_CreatesIndexFile()
    {
        // Arrange
        var projectPath = "../../../../Test.Project/Test.Project.csproj";
        var config = new TypeScriptProjectConfiguration();
        
        // Act
        var project = await _loader.GenerateProjectAsync(projectPath, config);
        
        // Assert
        Assert.That(project.ProjectFiles, Does.ContainKey("src/index.ts"));
        Assert.That(project.ProjectFiles["src/index.ts"], Does.Contain("export *"));
    }
}
