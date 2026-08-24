using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blazorise.Tests.Components;

public class ModalProviderComponentTest : BunitContext
{
    public ModalProviderComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider();
        Services.AddScoped<IModalService, ModalService>();
        JSInterop
            .AddBlazoriseButton()
            .AddBlazoriseModal()
            .AddBlazoriseClosable();
    }

    [Fact]
    public void TopMost_ShouldBeNull_WhenNoModalIsVisible()
    {
        var component = Render<ModalProvider>();

        Assert.Null( component.Instance.TopMost );
    }

    [Fact]
    public async Task TopMost_ShouldReturnMostRecentlyActivatedVisibleModal()
    {
        var component = Render<ModalProvider>();
        var modalService = Services.GetRequiredService<IModalService>();

        var firstModal = await ShowModal( component, modalService, "First" );
        var secondModal = await ShowModal( component, modalService, "Second" );

        Assert.Same( secondModal, component.Instance.TopMost );

        await component.InvokeAsync( () => modalService.Hide( secondModal ) );

        Assert.Same( firstModal, component.Instance.TopMost );
    }

    [Fact]
    public async Task TopMost_ShouldSkipClosedStatefulModal()
    {
        var component = Render<ModalProvider>( parameters => parameters
            .Add( x => x.Animated, false ) );
        var modalService = Services.GetRequiredService<IModalService>();

        var firstModal = await ShowModal( component, modalService, "First" );
        var secondModal = await ShowModal( component, modalService, "Second", new() { Stateful = true } );

        await component.InvokeAsync( () => modalService.Hide( secondModal ) );

        Assert.False( secondModal.Visible );
        Assert.Same( firstModal, component.Instance.TopMost );

        await component.InvokeAsync( () => modalService.Hide() );

        component.WaitForAssertion( () => Assert.Null( component.Instance.TopMost ) );
    }

    [Fact]
    public async Task Show_ShouldPromoteReopenedStatefulModal()
    {
        var component = Render<ModalProvider>();
        var modalService = Services.GetRequiredService<IModalService>();

        var firstModal = await ShowModal( component, modalService, "First", new() { Stateful = true } );
        var secondModal = await ShowModal( component, modalService, "Second" );

        await component.InvokeAsync( () => modalService.Hide( firstModal ) );
        await component.InvokeAsync( () => modalService.Show( firstModal ) );

        Assert.True( firstModal.Visible );
        Assert.Same( firstModal, component.Instance.TopMost );
        Assert.NotSame( secondModal, component.Instance.TopMost );
    }

    private static Task<ModalInstance> ShowModal( IRenderedComponent<ModalProvider> component, IModalService modalService, string title, ModalInstanceOptions options = null )
    {
        return component.InvokeAsync( () => modalService.Show( title, (RenderFragment)( builder => builder.AddContent( 0, title ) ), options ) );
    }
}