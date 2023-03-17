using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder( args );
builder.Logging.ClearProviders();

var app = builder.Build();

//app.UseWebAssemblyDebugging();
//app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapFallbackToFile( "index.html" );

app.Run();

public partial class Program { }