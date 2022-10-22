#region Using directives
using Blazorise.FluentValidation;
using Blazorise.LoadingIndicator;
using Blazorise.RichTextEdit;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Demo
{
    public static class Config
    {
        public static IServiceCollection SetupDemoServices( this IServiceCollection services )
        {
            services
                .AddBlazorise( options =>
                {
                    options.LicenseKey = "CjxRAnB+Nwo9UQRzfDE1BlEAc300CD5RAHF+NQg8bjoPL2hdNiFZLSIlf20XVTJDD3tjDkAgIHwCQQpTMyF2V2oXPUsMSAk4F0EECzxpSwFqCwJJWkMadBkHcgt0UGkALWhNPTRAeAlvCFoxSjUIUgtLN0MTeTx7SBZ0cjgyCWRWSRZ3bFlCKlEPCWYJVCRyChlnck81eDc+aUEjAHs5KlZiVBEIbgVKa2EXcQ0aMn5LL18VKGZUZBRZMh9fdV0sXBchcwo9NVIUG28OVlF/EwpREzUgfHgHYg1lM1Q2HHBTJzFWdhddCVkRSCQcVlJvOQAINFxPSyl9JyNRdkg6VxQ8THpGLFUkelRBRBZJFh9NSWUXaiR3VUpUNHItG21qWgEbMQM1SFs1cisWRn17CQQDHW8JaA99BBd/EyckGzkfVmFWIl4HG2BhbSB/InxvVTRMcyMFTUs4N3cqKGYKVAREAydmCXtUYyU9Y0JCM2EkCkxgSA1ZDRtfE3gOXCh9ZEI1DElwAz1Rblp6OCFoD1xRRS46cw5bVmF8cw==";
                    options.Immediate = true;
                } )
                .AddBlazoriseRichTextEdit( options =>
                {
                    options.UseBubbleTheme = true;
                    options.UseShowTheme = true;
                } )
                .AddLoadingIndicator()
                .AddBlazoriseFluentValidation();

            services.AddValidatorsFromAssembly( typeof( App ).Assembly );

            services.AddMemoryCache();

            // register demo services to fetch test data
            services.AddScoped<Shared.Data.EmployeeData>();
            services.AddScoped<Shared.Data.CountryData>();

            return services;
        }
    }
}
