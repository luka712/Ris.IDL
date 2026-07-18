namespace Ris.Idl.Typescript;

public class BaseTypeScriptGenerator
{
    /// <summary>
    /// Formats the name based on the case type.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="caseType">The case.</param>
    /// <returns></returns>
    public string FormatName(string name, Case caseType)
    {
        if (caseType == Case.Pascal)
        {
            return name.First().ToString().ToUpper() + name.Substring(1);
        }
        
        if (caseType == Case.Camel)
        {
            return name.First().ToString().ToLower() + name.Substring(1);
        }
        
        return name;
    }

    /// <summary>
    /// For some languages C# namespace becomes a module, for example in TypeScript.
    /// </summary>
    /// <param name="namespace">The C# namespace.</param>
    /// <param name="caseType">The case type.</param>
    /// <returns>The module namespace.</returns>
    public string FormatModule(string @namespace, Case caseType)
    {
        var parts = @namespace.Split('.');
        var module = String.Join('/', parts);
        
        if (caseType == Case.Lower)
        {
            return module.ToLower();
        }
        
        if (caseType == Case.Upper)
        {
            return module.ToUpper();
        }
        
        return module;
    }
    
    public string FormatFileName(string name)
    {
        return name.Replace('.', '_');
    }
    
}
