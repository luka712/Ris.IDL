using Ris.Idl.Core;
using Ris.Idl.Utilities;

namespace Ris.Idl.Test;

[TestFixture]
public class NamingHelperTests
{
    [TestCase("TestProperty", NamingCase.Camel, "testProperty")]
    [TestCase("TestProperty", NamingCase.Pascal, "TestProperty")]
    [TestCase("TestProperty", NamingCase.Snake, "test_property")]
    [TestCase("TestProperty", NamingCase.Kebab, "test-property")]
    [TestCase("TestProperty", NamingCase.Lower, "testproperty")]
    [TestCase("TestProperty", NamingCase.Upper, "TESTPROPERTY")]
    public void FormatName_ConvertsCorrectly(string input, NamingCase namingCase, string expected)
    {
        var result = NamingHelper.FormatName(input, namingCase);
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("testProperty", "TestProperty")]
    [TestCase("test_property", "TestProperty")]
    [TestCase("test-property", "TestProperty")]
    [TestCase("TestProperty", "TestProperty")]
    public void ToPascalCase_ConvertsCorrectly(string input, string expected)
    {
        var result = NamingHelper.ToPascalCase(input);
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("TestProperty", "testProperty")]
    [TestCase("test_property", "testProperty")]
    [TestCase("test-property", "testProperty")]
    [TestCase("testProperty", "testProperty")]
    public void ToCamelCase_ConvertsCorrectly(string input, string expected)
    {
        var result = NamingHelper.ToCamelCase(input);
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("TestProperty", "test_property")]
    [TestCase("testProperty", "test_property")]
    [TestCase("test-property", "test_property")]
    public void ToSnakeCase_ConvertsCorrectly(string input, string expected)
    {
        var result = NamingHelper.ToSnakeCase(input);
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("TestProperty", "test-property")]
    [TestCase("testProperty", "test-property")]
    [TestCase("test_property", "test-property")]
    public void ToKebabCase_ConvertsCorrectly(string input, string expected)
    {
        var result = NamingHelper.ToKebabCase(input);
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("Test.Project", NamingCase.Lower, "test/project")]
    [TestCase("Test.Project.Models", NamingCase.Lower, "test/project/models")]
    [TestCase("Test.Project", NamingCase.Pascal, "Test/Project")]
    public void NamespaceToModulePath_ConvertsCorrectly(string @namespace, NamingCase namingCase, string expected)
    {
        var result = NamingHelper.NamespaceToModulePath(@namespace, namingCase);
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("Test.Interface.ts", "Test_Interface.ts")]
    [TestCase("ITestInterface.ts", "ITestInterface.ts")]
    public void SanitizeFileName_HandlesDotsCorrectly(string input, string expected)
    {
        var result = NamingHelper.SanitizeFileName(input);
        Assert.That(result, Is.EqualTo(expected));
    }
}
