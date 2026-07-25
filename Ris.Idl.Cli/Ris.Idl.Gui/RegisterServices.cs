using Ris.Idl.Gui.Repository;
using Ris.Idl.Gui.Services;

namespace Ris.Idl.Gui;

/// <summary>
/// The services to register.
/// </summary>
public static class RegisterServices
{
    /// <summary>
    /// The services to register.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    public static void Register(IServiceCollection services)
    {
        services.AddTransient<RisIdlDbContext>();
        services.AddTransient<ConversionService>();
        services.AddTransient<ProjectGeneratorService>();
        services.AddTransient<ProjectLoader>();
        services.AddSingleton<AppLogger>();
    }
}