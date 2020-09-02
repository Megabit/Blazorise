#region Using directives
using System;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.RichTextEdit
{
    public static class RichTextEditComponent
    {
        /// <summary>
        /// Adds the Blazorise RichTextEdit extension related services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddBlazoriseRichTextEdit( this IServiceCollection services, Action<RichTextEditOptions> options = default )
        {
            var rteOptions = new RichTextEditOptions();

            options?.Invoke( rteOptions );

            services.AddSingleton( rteOptions );
            services.AddScoped<RichTextEditJsInterop>();

            return services;
        }
    }
}