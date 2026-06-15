using System;
using System.Linq;
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

        component.WaitForAssertion( () => Assert.Contains( "modal-lg", FindLatestModalDialog( component ).ClassName ) );
    }

    [Fact]
    public async Task InfoDoesNotReusePreviousSize()
    {
        var component = Render<MessageProvider>();

        await ShowInfoMessage( component, options => options.Size = ModalSize.Large );
        component.WaitForAssertion( () => Assert.Contains( "modal-lg", FindLatestModalDialog( component ).ClassName ) );

        await component.Find( "button" ).ClickAsync();

        await ShowInfoMessage( component );

        component.WaitForAssertion( () => Assert.DoesNotContain( "modal-lg", FindLatestModalDialog( component ).ClassName ) );
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

    [Fact]
    public async Task ConfirmCanBeChainedFromCancelContinuation()
    {
        var component = Render<MessageProvider>();
        var messageService = Services.GetRequiredService<IMessageService>();
        Task flowTask = null;
        Task<bool> secondConfirmTask = null;

        async Task ShowMessages()
        {
            if ( await messageService.Confirm( "Confirm1", "Title" ) )
            {
                return;
            }

            secondConfirmTask = messageService.Confirm( "Confirm2", "Title" );

            await secondConfirmTask;
        }

        await component.InvokeAsync( () =>
        {
            flowTask = ShowMessages();

            return Task.CompletedTask;
        } );

        component.WaitForAssertion( () => Assert.Contains( "Confirm1", component.Markup ) );

        await ClickLatestCancelButton( component );

        component.WaitForAssertion( () => Assert.Contains( "Confirm2", component.Markup ) );
        Assert.NotNull( secondConfirmTask );
        Assert.False( secondConfirmTask.IsCompleted );

        await ClickLatestCancelButton( component );
        await flowTask;

        Assert.False( await secondConfirmTask );
    }

    private static Task ClickLatestCancelButton( IRenderedComponent<MessageProvider> component )
    {
        var buttons = component.FindAll( "button" );

        return buttons[buttons.Count - 2].ClickAsync();
    }

    private static AngleSharp.Dom.IElement FindLatestModalDialog( IRenderedComponent<MessageProvider> component )
    {
        var modalDialogs = component.FindAll( ".modal-dialog" );

        return modalDialogs.Last();
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