using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Blazorise.Demo.Bootstrap5.Server
{
    public class Program
    {
        public static void Main( string[] args )
        {
            BuildWebHost( args ).Run();
        }

        public static IWebHost BuildWebHost( string[] args ) =>
            WebHost.CreateDefaultBuilder( args )
                .UseConfiguration( new ConfigurationBuilder()
                    .AddCommandLine( args )
                    .Build() )
                .UseStartup<Startup>()
                .Build();
    }
}
