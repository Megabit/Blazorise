using Microsoft.AspNetCore.Components;
using Moq;

namespace Blazorise.Tests.Mocks
{
    internal class MockButton : Button
    {
        public MockButton( Dropdown parentDropdown = null, Addons parentAddons = null, Buttons parentButtons = null )
        {
            var mockRunner = new Mock<IJSRunner>();
            mockRunner.Setup( r => r.Focus( It.IsAny<ElementReference>(), It.IsAny<string>(), It.IsAny<bool>() ) )
                      .Callback( ( ElementReference r, string i, bool s ) => this.OnFocusCalled( r, i, s ) );
            this.JSRunner = mockRunner.Object;

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

        public void Click()
        {
            this.ClickHandler();
        }

        private bool OnFocusCalled( ElementReference elementReference, string elementId, bool scrollToElement )
        {
            this.FocusedId = elementId;
            return true;
        }
    }
}
