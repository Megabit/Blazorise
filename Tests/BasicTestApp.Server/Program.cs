using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder( args );

var app = builder.Build();

app.UseWebAssemblyDebugging();

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapFallbackToFile( "index.html" );

app.Run();

public partial class Program { }