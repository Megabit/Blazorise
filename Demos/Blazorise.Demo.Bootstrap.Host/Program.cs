using Blazorise.Bootstrap;
using Blazorise.Demo;
using Blazorise.Icons.FontAwesome;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services
        .SetupDemoServices()
        .AddBootstrapProviders()
        .AddFontAwesomeIcons();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseEndpoints( endpoints =>
{
    endpoints.MapRazorPages(); 
    endpoints.MapFallbackToPage( "/_Host" ); 
} );

app.Run();
