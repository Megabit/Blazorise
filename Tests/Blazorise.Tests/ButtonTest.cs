#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Tests.Mocks;
using Microsoft.AspNetCore.Components;
using Xunit;
#endregion

namespace Blazorise.Tests
{
    public class ButtonTest
    {
        private readonly EventCallbackFactory callbackFactory = new EventCallbackFactory();

        [Fact]
        public void SetFocus()
        {
            // setup
            var button = new MockButton();
            var expectedId = button.ElementId;

            // test
            button.Focus();

            // validate
            Assert.Equal( expectedId, button.FocusedId );
        }

        [Fact]
        public void SetMargin()
        {
            // setup
            var margin = new FluentMargin().Is3;
            var button = new Button();

            // test
            button.Margin = margin;

            // validate
            Assert.Equal( margin, button.Margin );
        }

        [Fact]
        public void SetPadding()
        {
            // setup
            var padding = new FluentPadding().Is1;
            var button = new Button();

            // test
            button.Padding = padding;

            // validate
            Assert.Equal( padding, button.Padding );
        }

        [Fact]
        public void SetSize()
        {
            // setup
            var button = new Button();

            // test
            button.Size = Size.Small;

            // validate
            Assert.Equal( Size.Small, button.Size );
        }

        [Fact]
        public void SetParentDropdown()
        {
            // setup
            var drop = new Dropdown();
            var button = new MockButton( drop );

            // test
            button.Dispose();

            // validate
        }

        [Fact]
        public void SetParentAddons()
        {
            // setup
            var a = new Addons();
            var button = new MockButton( parentAddons: a );

            // test
            button.Dispose();

            // validate
            Assert.False( button.IsAddons );
        }

        [Fact]
        public void SetParentButtons()
        {
            // setup
            var b = new Buttons();
            var button = new MockButton( parentButtons: b );

            // test
            button.Dispose();

            // validate
            Assert.True( button.IsAddons );
        }

        [Fact]
        public async Task ClickWithEventCallback()
        {
            // setup
            var button = new MockButton();
            bool clicked = false;

            // test
            button.Clicked = callbackFactory.Create( this, () => { clicked = true; } );
            await button.Click();

            // validate
            Assert.True( clicked );
        }

        [Fact]
        public async Task ClickWithCommand()
        {
            // setup
            var button = new MockButton();
            string result = null;
            button.Command = new TestCommand( p => result = p );
            button.CommandParameter = "foo";

            // test
            await button.Click();

            // validate
            Assert.NotNull( result );
            Assert.Equal( "foo", result );
        }

        class TestCommand : System.Windows.Input.ICommand
        {
            private readonly Action<string> callback;

            public TestCommand( Action<string> callback )
            {
                this.callback = callback;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute( object parameter )
            {
                return true;
            }

            public void Execute( object parameter )
            {
                var result = parameter.ToString() ?? "NoParam";
                this.callback.Invoke( result );
            }
        }
    }
}
