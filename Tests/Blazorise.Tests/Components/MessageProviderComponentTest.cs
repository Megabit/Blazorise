using System;
using System.Threading.Tasks;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blazorise.Tests.Components;

public class MessageProviderComponentTest : BunitContext
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
        var component = Render<MessageProvider>();

        await ShowInfoMessage( component, options => options.Size = ModalSize.Large );

        component.WaitForAssertion( () => Assert.Contains( "modal-lg", component.Find( ".modal-dialog" ).ClassName ) );
    }

    [Fact]
    public async Task InfoDoesNotReusePreviousSize()
    {
        var component = Render<MessageProvider>();

        await ShowInfoMessage( component, options => options.Size = ModalSize.Large );
        component.WaitForAssertion( () => Assert.Contains( "modal-lg", component.Find( ".modal-dialog" ).ClassName ) );

        await component.Find( "button" ).ClickAsync();

        await ShowInfoMessage( component );

        component.WaitForAssertion( () => Assert.DoesNotContain( "modal-lg", component.Find( ".modal-dialog" ).ClassName ) );
    }

    [Fact]
    public async Task ConfirmCloseOnEscapeCompletesAsCanceled()
    {
        var component = Render<TestMessageProvider>();
        var messageService = Services.GetRequiredService<IMessageService>();
        Task<bool> confirmTask = null;

        await component.InvokeAsync( () =>
        {
            confirmTask = messageService.Confirm( "Message", "Title", options => options.CloseOnEscape = true );

            return Task.CompletedTask;
        } );

        Assert.NotNull( confirmTask );
        component.WaitForAssertion( () => Assert.Contains( "Message", component.Markup ) );

        await component.InvokeAsync( () => component.Instance.InvokeModalClosing( new( false, CloseReason.EscapeClosing ) ) );

        Assert.False( await confirmTask );
    }

    private Task ShowInfoMessage( IRenderedComponent<MessageProvider> component, Action<MessageOptions> options = null )
    {
        var messageService = Services.GetRequiredService<IMessageService>();

        return component.InvokeAsync( () => messageService.Info( "Message", "Title", options ) );
    }

    private class TestMessageProvider : MessageProvider
    {
        public Task InvokeModalClosing( ModalClosingEventArgs eventArgs )
            => OnModalClosing( eventArgs );
    }
}