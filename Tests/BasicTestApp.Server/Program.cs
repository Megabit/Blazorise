
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace BasicTestApp.Server;
public partial class Program
{
    public static void Main( string[] args )
    {
        var builder = WebApplication.CreateBuilder( args );
        builder.Logging.ClearProviders();

        WebApplication app = builder.Build();

        //app.UseWebAssemblyDebugging();
        //app.UseHttpsRedirection();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.MapFallbackToFile( "index.html" );

        app.Run();
    }
}

public partial class Program { }

