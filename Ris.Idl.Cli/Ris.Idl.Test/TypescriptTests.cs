using Microsoft.CodeAnalysis;
using Ris.Idl.Typescript;

namespace Ris.Idl.Test;

public class Tests
{
    private readonly TypeScriptInterfaceGenerator _interfaceGenerator = new();
    private readonly TypeScriptClassGenerator _classGenerator = new();
    
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public async Task Test_And_Generate_Project()
    {
        var loader = new Loader();
        var project = await loader.LoadProjectAsync("../../../../Test.Project/Test.Project.csproj");
        
        // Find the ITestInterface
        var iface = project.Interfaces.FirstOrDefault(i => i.Name == "ITestInterface");
        
        Assert.That(iface, Is.Not.Null);
    }
    
    // [Test]
    // public void InterfaceWithPropertiesOnly_Test()
    // {
    //     var typescriptCode = _interfaceGenerator.Convert(File.ReadAllText("TestInterfaces/InterfaceWithPropertiesOnly.cs"));
    //    
    //     Assert.That(typescriptCode, Is.Not.Null);
    // }
    //
    // [Test]
    // public void ClassWithPropertiesOnly_Test()
    // {
    //    var typescriptCode = _classGenerator.Convert(File.ReadAllText("TestClasses/ClassWithPropertiesOnly.cs"));
    //    
    //    Assert.That(typescriptCode, Is.Not.Null);
    // }
}