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
        private readonly EventCallbackFactory callbackFactory = new();

        [Fact]
        public async Task SetFocus()
        {
            // setup
            var button = new MockButton();
            var expectedId = button.ElementId;

            // test
            await button.Focus();

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
        public async Task SetParentDropdown()
        {
            // setup
            var drop = new Dropdown();
            var button = new MockButton( drop );

            // test
            await button.DisposeAsync();

            // validate
        }

        [Fact]
        public async Task SetParentAddons()
        {
            // setup
            var a = new Addons();
            var button = new MockButton( parentAddons: a );

            // test
            await button.DisposeAsync();

            // validate
            Assert.False( button.IsAddons );
        }

        [Fact]
        public async Task SetParentButtons()
        {
            // setup
            var b = new Buttons();
            var button = new MockButton( parentButtons: b );

            // test
            await button.DisposeAsync();

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
            button.CommandParameter = new TestCommandParameter { Message = "foo" };

            // test
            await button.Click();

            // validate
            Assert.NotNull( result );
            Assert.Equal( "foo", result );
        }

        [Fact]
        public void CommandCantExecuteDisablesButton()
        {
            var button = new MockButton();
            Assert.False( button.Disabled );

            var command = new TestCommand( _ => { } );
            var parameter = new TestCommandParameter { Message = "foo" };

            button.Command = command;
            Assert.True( button.Disabled );

            button.CommandParameter = parameter;
            Assert.False( button.Disabled );

            command.FireCanExecuteChanged();
            Assert.False( button.Disabled );

            parameter.Message = null;
            command.FireCanExecuteChanged();
            Assert.True( button.Disabled );

            button.Command = null;
            Assert.False( button.Disabled );
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
                => parameter is TestCommandParameter param && !string.IsNullOrWhiteSpace( param.Message );

            public void Execute( object parameter )
            {
                var result = parameter is TestCommandParameter param ? param.Message : "NoParam";
                this.callback.Invoke( result );
            }

            public void FireCanExecuteChanged() => CanExecuteChanged?.Invoke( this, EventArgs.Empty );
        }

        class TestCommandParameter
        {
            public string Message { get; set; }
        }
    }
}
