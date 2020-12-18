#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Bootstrap;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Material
{
    public static class Config
    {
        public static IServiceCollection AddMaterialProviders( this IServiceCollection serviceCollection, Action<IClassProvider> configureClassProvider = null )
        {
            var classProvider = new MaterialClassProvider();

            configureClassProvider?.Invoke( classProvider );

            serviceCollection.AddSingleton<IClassProvider>( classProvider );
            serviceCollection.AddSingleton<IStyleProvider, MaterialStyleProvider>();
            serviceCollection.AddScoped<IJSRunner, MaterialJSRunner>();
            serviceCollection.AddScoped<IThemeGenerator, MaterialThemeGenerator>();

            serviceCollection.AddBootstrapComponents();

            // material overrides
            serviceCollection.AddTransient( typeof( Blazorise.Switch<> ), typeof( Material.Switch<> ) );
            serviceCollection.AddTransient<Blazorise.Step, Material.Step>();
            serviceCollection.AddTransient<Blazorise.Steps, Material.Steps>();

            return serviceCollection;
        }
    }
}
