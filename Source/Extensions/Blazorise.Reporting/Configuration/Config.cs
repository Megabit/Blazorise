#region Using directives
using System;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Reporting;

public static class Config
{
    /// <summary>
    /// Adds the Blazorise Reporting extension related services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="options">The reporting options configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddBlazoriseReporting( this IServiceCollection services, Action<ReportOptions> options = default )
    {
        var reportOptions = new ReportOptions();

        options?.Invoke( reportOptions );

        services.AddSingleton( reportOptions );

        return services;
    }
}