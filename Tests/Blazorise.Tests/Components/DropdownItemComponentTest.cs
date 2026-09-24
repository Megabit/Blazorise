#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class DropdownItemComponentTest : BunitContext
{
    public DropdownItemComponentTest()
    {
        Services.AddBlazoriseTests().AddEmptyIconProvider();
        JSInterop.AddBlazoriseUtilities();
    }

    [Fact]
    public void Indeterminate_Should_Set_And_Clear_Checkbox_Property()
    {
        Services.AddBootstrapProviders();

        var comp = Render<DropdownItem>( parameters => parameters
            .Add( x => x.ShowCheckbox, true )
            .Add( x => x.Checked, true )
            .Add( x => x.Indeterminate, true ) );

        comp.WaitForAssertion( () =>
        {
            var invocation = JSInterop.VerifyInvoke( "setProperty" );
            Assert.Equal( "indeterminate", invocation.Arguments[1] );
            Assert.Equal( true, invocation.Arguments[2] );
        } );

        comp.Render( parameters => parameters.Add( x => x.Indeterminate, false ) );

        comp.WaitForAssertion( () =>
        {
            Assert.Equal( 2, JSInterop.Invocations["setProperty"].Count );
            var invocation = JSInterop.Invocations["setProperty"].Last();
            Assert.Equal( "indeterminate", invocation.Arguments[1] );
            Assert.Equal( false, invocation.Arguments[2] );
            Assert.True( comp.FindComponent<Check<bool>>().Instance.Value );
        } );
    }

    [Theory]
    [InlineData( false )]
    [InlineData( true )]
    public async Task Indeterminate_Should_Preserve_Checkbox_Activation_And_Disabled_Behavior( bool disabled )
    {
        Services.AddBootstrapProviders();
        var changes = new List<bool>();
        var comp = Render<DropdownItem>( parameters => parameters
            .Add( x => x.ShowCheckbox, true )
            .Add( x => x.Indeterminate, true )
            .Add( x => x.Disabled, disabled )
            .Add( x => x.CheckedChanged, changes.Add ) );

        await comp.Find( "a > div" ).MouseDownAsync( new() );

        Assert.Equal( disabled ? 0 : 1, changes.Count );
        Assert.True( changes.All( value => value ) );
        Assert.Equal( !disabled, comp.FindComponent<Check<bool>>().Instance.Value );
        Assert.True( comp.Instance.Indeterminate );
    }
}