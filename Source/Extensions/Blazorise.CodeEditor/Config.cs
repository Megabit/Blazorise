#region Using directives
using System;
using Microsoft.Extensions.DependencyInjection;
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
        var codeEditorOptions = new CodeEditorExtensionOptions();

        options?.Invoke( codeEditorOptions );

        services.AddSingleton( codeEditorOptions );
        services.AddScoped<JSCodeEditorModule>();

        return services;
    }
}