using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;

public class TemplateComponentTest : TestContext
{
    public TemplateComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
    }

    [Fact]
    public void RenderTest()
    {
        // setup

        // test

        // validate

    }
}