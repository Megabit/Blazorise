using System;
using System.Threading.Tasks;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blazorise.Tests.Components;

public class MessageProviderComponentTest : TestContext
{
    public MessageProviderComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider();
        JSInterop
            .AddBlazoriseButton()
            .AddBlazoriseModal()
            .AddBlazoriseClosable();
    }

    [Fact]
    public async Task InfoAppliesRequestedSizeBeforeOpening()
    {
        var component = RenderComponent<MessageProvider>();

        await ShowInfoMessage( component, options => options.Size = ModalSize.Large );

        component.WaitForAssertion( () => Assert.Contains( "modal-lg", component.Find( ".modal-dialog" ).ClassName ) );
    }

    [Fact]
    public async Task InfoDoesNotReusePreviousSize()
    {
        var component = RenderComponent<MessageProvider>();

        await ShowInfoMessage( component, options => options.Size = ModalSize.Large );
        component.WaitForAssertion( () => Assert.Contains( "modal-lg", component.Find( ".modal-dialog" ).ClassName ) );

        await component.Find( "button" ).ClickAsync();

        await ShowInfoMessage( component );

        component.WaitForAssertion( () => Assert.DoesNotContain( "modal-lg", component.Find( ".modal-dialog" ).ClassName ) );
    }

    private Task ShowInfoMessage( IRenderedComponent<MessageProvider> component, Action<MessageOptions> options = null )
    {
        var messageService = Services.GetRequiredService<IMessageService>();

        return component.InvokeAsync( () => messageService.Info( "Message", "Title", options ) );
    }
}