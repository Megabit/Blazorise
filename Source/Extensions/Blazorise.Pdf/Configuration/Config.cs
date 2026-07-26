#region Using directives
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Provides service registration helpers for Blazorise PDF generation.
/// </summary>
public static class Config
{
    #region Methods

    /// <summary>
    /// Adds the Blazorise PDF generation services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddBlazorisePdf( this IServiceCollection services )
    {
        services.TryAddScoped<IPdfGenerator, PdfGenerator>();
        services.TryAddScoped<IPdfRenderProvider, SimplePdfRenderProvider>();

        return services;
    }

    #endregion
}