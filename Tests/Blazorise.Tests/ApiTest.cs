#region Using directives
using System.Threading.Tasks;
using Blazorise;
using PublicApiGenerator;
using VerifyXunit;
using Xunit;
#endregion

public class ApiTest
{
    [Fact]
    public Task Run()
    {
        var options = new ApiGeneratorOptions
        {
            IncludeAssemblyAttributes = false
        };
        var publicApi = typeof(BlazoriseOptions)
            .Assembly
            .GeneratePublicApi(options:options);

        return Verifier.Verify(publicApi);
    }
}