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
                    options.LicenseKey = "CjxRAnB+Nwo8UgF5fD01BlEAc300CD5RAHF+NQg8bjoPL2hdNiFZLSIlf20XVTJDD3tjDkAgIHwCQQpTMyF2V2oXPUsPM2FPW0cYHE1iQBB8CCJrWz8zci99UwBADEUoI3UIYjEEdRRGf14UQAwdVhM9E1oAfXxcRSIAKTlHSUQiWgxhc3k0IlsqF29SbkxXdSNtbz4VShYeN1xOV1obDWIATVRqNjc1Ym8NeXY2VgtLJmY1I0kIXBBEJwc1cUAmZwA3Kg9IIngnP0gBRi5oagVcClgvUQ0vRH9/DHdwBkcIfzZpbhxtDXgUVAcHPHlYVwUMGWFsRVRpLDZMXmIAWQV+aU58E1E7AS56fwJfOCRkclkHYnEgcVR7UXspFkdRdFt0DCd1YXorXigadmw6NGcGelcJWgdUEB5/S1sLd3l8anpjUXIUKWlufCtqCBxDW24yVRYqQlVuNV4lIGNdOiF1FhZTADoJZDItTwltKkk1e3B1bldqOH03UlYuVQ4nYg5VMkcieipPPiBfFS88ADQBBC4GXFFhUld8cw==";
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
