using Blazorise.Demo.Bootstrap.RC;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder( args )
    .ConfigureWebHostDefaults( webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    } )
    .Build()
    .Run();