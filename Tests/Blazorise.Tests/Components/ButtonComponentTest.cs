#region Using directives
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class ButtonComponentTest : TestContext
{
    public ButtonComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop
            .AddBlazoriseButton();
    }

    [Fact]
    public void Render_Should_Invoke_Initialize_When_PreventDefaultOnSubmit()
    {
        // setup
        var buttonOpen = "<button";
        var buttonClose = "</button>";

        // test
        var comp = RenderComponent<Button>( parameters =>
        parameters.Add( x => x.PreventDefaultOnSubmit, true ) );

        // validate
        this.JSInterop.VerifyInvoke( "initialize" );
        Assert.Contains( buttonOpen, comp.Markup );
        Assert.Contains( buttonClose, comp.Markup );
    }

    [Fact]
    public void RenderTest()
    {
        // setup
        var buttonOpen = "<button";
        var buttonClose = "</button>";
        var buttonContent = "Count";
        var counterOutput = @"<span id=""basic-button-event-result"">0</span>";

        // test
        var comp = RenderComponent<ButtonComponent>();

        // validate
        this.JSInterop.VerifyNotInvoke( "initialize" );
        Assert.Contains( buttonOpen, comp.Markup );
        Assert.Contains( buttonClose, comp.Markup );
        Assert.Contains( buttonContent, comp.Markup );
        Assert.Contains( counterOutput, comp.Markup );
        Assert.NotNull( comp.Find( "#basic-button-event" ) );
        Assert.NotNull( comp.Find( "#basic-button" ) );
        Assert.NotNull( comp.Find( "#basic-button-event-result" ) );
    }


    [Fact]
    public async Task CanRaiseCallback()
    {
        // setup
        var comp = RenderComponent<ButtonComponent>();

        var result = comp.Find( "#basic-button-event-result" );
        var button = comp.Find( "#basic-button" );

        // test
        await button.ClickAsync();
        var result1 = result.InnerHtml;

        await button.ClickAsync();
        var result2 = result.InnerHtml;

        // validate
        this.JSInterop.VerifyNotInvoke( "initialize" );
        Assert.Equal( "1", result1 );
        Assert.Equal( "2", result2 );
    }

    [Fact]
    public void Render_ShouldRefreshDisabledState_When_Command_BecomesExecutable_During_FirstRenderCycle()
    {
        // setup
        var command = new ToggleCommand( canExecute: false );

        // test
        var comp = RenderComponent<FirstRenderCanExecuteChangedButton>( parameters => parameters
            .Add( x => x.Command, command )
            .AddChildContent( "Click" ) );

        // validate
        comp.WaitForAssertion( () => Assert.False( comp.Instance.Disabled ) );
        Assert.False( comp.Find( "button" ).HasAttribute( "disabled" ) );
    }

    private sealed class FirstRenderCanExecuteChangedButton : Button
    {
        protected override Task OnFirstAfterRenderAsync()
        {
            if ( Command is ToggleCommand command )
            {
                command.SetCanExecute( true );
                command.FireCanExecuteChanged();
            }

            return Task.CompletedTask;
        }
    }

    private sealed class ToggleCommand : ICommand
    {
        private bool canExecute;

        public ToggleCommand( bool canExecute )
        {
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute( object parameter ) => canExecute;

        public void Execute( object parameter )
        {
        }

        public void SetCanExecute( bool canExecute )
        {
            this.canExecute = canExecute;
        }

        public void FireCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke( this, EventArgs.Empty );
        }
    }
}