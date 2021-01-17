#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Icons.FontAwesome
{
    public static class Config
    {
        public static IServiceCollection AddFontAwesomeIcons( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IIconProvider, FontAwesomeIconProvider>();

            serviceCollection.AddTransient<Blazorise.Icon, FontAwesome.Icon>();

            return serviceCollection;
        }
    }
}
