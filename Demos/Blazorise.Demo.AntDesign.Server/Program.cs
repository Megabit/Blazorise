using Blazorise.Demo.AntDesign.Server;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

WebHost.CreateDefaultBuilder( args )
    .UseConfiguration( new ConfigurationBuilder()
        .AddCommandLine( args )
        .Build() )
    .UseStartup<Startup>()
    .Build()
    .Run();