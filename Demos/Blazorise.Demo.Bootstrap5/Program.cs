#region Using directives
using System.Threading.Tasks;
using Blazorise.Bootstrap5;
using Blazorise.Icons.Bootstrap;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
#endregion

namespace Blazorise.Demo.Bootstrap5;

public class Program
{
    public static async Task Main( string[] args )
    {
        var builder = WebAssemblyHostBuilder.CreateDefault( args );

        builder.Services
            .SetupDemoServices( "CjxRBnB/Nwg9UwJ2fzM1BlEAc3k0CT5TAHh/PAg1bjoNJ2ZdYhBVCCo/DTlWAExERldhE1EvN0xcNm46FD1gSkUHCnUsMFlqWgN4Y2YLaVcdJ380CSFbBnUvKAk8VVYldzUMNAIJeUMPdGUAVS89YGx1E1V7B2tcZQY9SwJMe0ktYwQRSHdIInwIGlwCRQ1UKCsIMkknZQIPUXFDLW8TAUl9Nm46BApQe003eQ4AWmtPLGAEdAgyUzx0ABpEf14qdB4RSHlUPG8TAVJrUzwKDwFadEUueRUdCDJTPHEUGkp7Qy5gDQtRfVM8fQAWWmdeLGcSEVoCQixvDQdIcVgwPUsRWntEImIVHVpnQSJoHhFXd1swbx50S3dTL3kMB1FrAWlvHgJMa1g1eQQZWmdBImgeEVd3WzBvHnRLd1MveQwHUWsBaW8eGld9STV5BBlaZ0EiaB4RV3dbMG8edEt3Uy95DAdRawFpYBMBT31PN28PD0h9NjdVMjpVSmMJVSI6CDJNM2ANB0Z5WCp/DxFEa18mfQMCTH1fWWtjDGlZdgxCKD1gFkgGXS5sWDUGLVYFHipLWAhmL3lLeU8WegQ9Z1toL0kHfnd7Pi1UOS9NYjoSZSJ7M3Q+VgQWKGhKVVZhCSFtUU4uYyodYXl7J0N4HH9RRAliJSFwTn4KRyIHbHV5Cl02FFB1VhEABnt9b207XSYFbVM1AAkZeFBfawZ/CXphfnYyQxAINwF2GlQFImxXYSkGFDc1CUVUQQwpcm9IM38QGmFpfxBVKCVoT3ZRVGoUXWhmCUoMcw==", builder.Configuration["ReCaptchaSiteKey"] )
            .AddBootstrap5Providers()
            .AddBootstrapIcons();

        builder.RootComponents.Add<WasmApp>( "#app" );
        var host = builder.Build();

        await host.RunAsync();
    }
}