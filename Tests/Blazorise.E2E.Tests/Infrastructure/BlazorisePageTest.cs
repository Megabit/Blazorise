using Microsoft.AspNetCore.Components;

namespace Blazorise.E2E.Tests.Infrastructure;


public class BlazorisePageTest : BlazorPageTest
{
    /// <summary>
    /// Override browser context options if needed.
    /// </summary>
    /// <returns></returns>
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions()
        {
            Locale = "en-US"
        };
    }

    /// <summary>
    /// This is an helper specific to our test project, where we have a dropdown selection with the full name of the components.
    /// This will also navigate to the root page.
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <returns></returns>
    protected async Task SelectTestComponent<TComponent>() where TComponent : IComponent
    {
        await Page.GotoAsync( RootUri.AbsoluteUri );

        var componentTypeName = typeof( TComponent ).FullName;
        await Page.GetByRole( AriaRole.Combobox ).SelectOptionAsync( new[] { componentTypeName } );
    }


}
