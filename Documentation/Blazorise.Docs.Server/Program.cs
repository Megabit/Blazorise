using Blazorise.Docs.Server;
using Microsoft.AspNetCore.Builder;

namespace Blazorise.Docs;

public class Program
{
    public static void Main( string[] args )
    {
        var builder = WebApplication.CreateBuilder( args );

        var startup = new Startup( builder.Configuration );

        startup.ConfigureServices( builder.Services );

        var app = builder.Build();

        startup.Configure( app );

        app.Run();
    }
}