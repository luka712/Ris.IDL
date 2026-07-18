namespace Ris.Idl;

public enum Case
{
    Pascal,
    Camel,
    Snake,
    Lower,
    Upper,
}

public class Config
{
    public Case PropertyCase { get; set; } = Case.Pascal;
    
    public Case ModuleCase { get; set; } = Case.Pascal;
}

public class TypescriptConfig : Config
{
    public TypescriptConfig()
    {
        // Typescript properties are converted to camel case. ie: ThisProperty becomes thisProperty.
        PropertyCase = Case.Camel;
        
        // All modules are converted to lower case.
        // For example Ris.Gui will becomes ris/gui in Typescript.
        ModuleCase = Case.Lower;
    }
}