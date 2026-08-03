#region Using directives
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
#endregion

namespace Blazorise.CodeEditor;

public static class Config
{
    /// <summary>
    /// Adds the Blazorise CodeEditor extension related services.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="options">Code editor extension options.</param>
    /// <returns>Service collection.</returns>
    public static IServiceCollection AddBlazoriseCodeEditor( this IServiceCollection services, Action<CodeEditorExtensionOptions> options = default )
    {
        CodeEditorExtensionOptions codeEditorOptions = new();

        if ( options is null )
        {
            services.TryAddSingleton( codeEditorOptions );
        }
        else
        {
            options.Invoke( codeEditorOptions );
            services.AddSingleton( codeEditorOptions );
        }

        services.TryAddScoped<JSCodeEditorModule>();

        return services;
    }
}