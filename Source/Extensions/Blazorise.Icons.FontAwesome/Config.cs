#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if NETCORE3_0
using Microsoft.AspNetCore.Builder;
#endif
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Icons.FontAwesome
{
    public static class Config
    {
        public static IServiceCollection AddFontAwesomeIcons( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IIconProvider, FontAwesomeIconProvider>();

            return serviceCollection;
        }

#if NETSTANDARD2_0

        public static IComponentsApplicationBuilder UseFontAwesomeIcons( this IComponentsApplicationBuilder app )
        {
            var componentMapper = app.Services.GetRequiredService<IComponentMapper>();

            componentMapper.Register<Blazorise.Icon, FontAwesome.Icon>();

            return app;
        }

#else

        public static IApplicationBuilder UseFontAwesomeIcons( this IApplicationBuilder app )
        {
            var componentMapper = app.ApplicationServices.GetRequiredService<IComponentMapper>();

            componentMapper.Register<Blazorise.Icon, FontAwesome.Icon>();

            return app;
        }

#endif
    }
}
