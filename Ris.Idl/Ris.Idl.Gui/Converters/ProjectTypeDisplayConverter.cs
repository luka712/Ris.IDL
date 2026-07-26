using Microsoft.UI.Xaml.Data;
using Ris.Idl.Gui.Models;

namespace Ris.Idl.Gui.Converters;

/// <summary>
/// Converts ProjectType enum values to display-friendly strings.
/// </summary>
public class ProjectTypeDisplayConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is ProjectType projectType)
        {
            return projectType switch
            {
                ProjectType.FullProject => "Full Project (with package.json, tsconfig.json)",
                ProjectType.IdlOnly => "IDL Only (interfaces without project structure)",
                _ => projectType.ToString()
            };
        }
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
