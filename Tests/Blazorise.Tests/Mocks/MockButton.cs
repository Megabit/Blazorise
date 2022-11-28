#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Moq;
#endregion

namespace Blazorise.Tests.Mocks;

internal class MockButton : Button
{
    public MockButton( Dropdown parentDropdown = null, Addons parentAddons = null, Buttons parentButtons = null )
    {
        var mockComponentDisposer = new Mock<IComponentDisposer>();
        this.ComponentDisposer = mockComponentDisposer.Object;

        mockComponentDisposer
            .Setup( r => r.Dispose( It.IsAny<BaseAfterRenderComponent>() ) );

        var mockRunner = new Mock<IJSUtilitiesModule>();

        mockRunner
            .Setup( r => r.Focus( It.IsAny<ElementReference>(), It.IsAny<string>(), It.IsAny<bool>() ) )
            .Callback( ( ElementReference r, string i, bool s ) => this.OnFocusCalled( r, i, s ) );

        this.JSUtilitiesModule = mockRunner.Object;

        var mockIdGenerator = new Mock<IIdGenerator>();

        mockIdGenerator
            .Setup( r => r.Generate )
            .Returns( Guid.NewGuid().ToString() );

        base.IdGenerator = mockIdGenerator.Object;

        this.ParentDropdown = parentDropdown;
        this.ParentAddons = parentAddons;
        this.ParentButtons = parentButtons;

        this.OnInitialized();
    }

    public string FocusedId { get; private set; } = null;

    public new bool IsAddons
    {
        get { return base.IsAddons; }
    }

    public Task Click()
    {
        return ClickHandler( new MouseEventArgs() );
    }

    private bool OnFocusCalled( ElementReference elementReference, string elementId, bool scrollToElement )
    {
        this.FocusedId = elementId;
        return true;
    }
}