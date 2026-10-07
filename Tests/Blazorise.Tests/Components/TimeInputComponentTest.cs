using System;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Blazorise.Tests.Components;

public class TimeInputComponentTest : BunitContext
{
    public TimeInputComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseUtilities();
    }

    [Theory]
    [InlineData( "01:05", 0 )]
    [InlineData( "01:05:07", 7 )]
    public async Task NativeTimeInputPreservesEditingValueUntilCompletedOrReplaced( string inputValue, int seconds )
    {
        // setup
        TimeOnly value = new( 15, 30 );
        IRenderedComponent<TimeInput<TimeOnly>> comp = Render<TimeInput<TimeOnly>>( parameters => parameters
            .Add( x => x.Value, value )
            .Add( x => x.ValueChanged, changedValue => value = changedValue )
            .Add( x => x.Step, 1 ) );

        Assert.Equal( "time", comp.Find( "input" ).GetAttribute( "type" ) );
        Assert.Equal( "15:30:00", comp.Find( "input" ).GetAttribute( "value" ) );

        // test
        await comp.Find( "input" ).ChangeAsync( new ChangeEventArgs { Value = string.Empty } );
        comp.Render( parameters => parameters.Add( x => x.Value, value ) );

        // validate existing default-value behavior without writing it back into the input
        Assert.Equal( default, value );
        Assert.Equal( string.Empty, comp.Find( "input" ).GetAttribute( "value" ) );

        // test
        await comp.Find( "input" ).ChangeAsync( new ChangeEventArgs { Value = inputValue } );
        comp.Render( parameters => parameters.Add( x => x.Value, value ) );

        // validate the native string is preserved, including optional seconds
        Assert.Equal( new TimeOnly( 1, 5, seconds ), value );
        Assert.Equal( inputValue, comp.Find( "input" ).GetAttribute( "value" ) );

        // test
        comp.Render( parameters => parameters.Add( x => x.Value, new TimeOnly( 16, 45 ) ) );

        // validate
        Assert.Equal( "16:45:00", comp.Find( "input" ).GetAttribute( "value" ) );
    }

    [Fact]
    public async Task NullableNativeTimeInputPreservesEmptyEditingValueAcrossRenders()
    {
        // setup
        TimeSpan? value = new TimeSpan( 15, 30, 0 );
        IRenderedComponent<TimeInput<TimeSpan?>> comp = Render<TimeInput<TimeSpan?>>( parameters => parameters
            .Add( x => x.Value, value )
            .Add( x => x.ValueChanged, changedValue => value = changedValue ) );

        // test
        await comp.Find( "input" ).ChangeAsync( new ChangeEventArgs { Value = string.Empty } );
        comp.Render( parameters => parameters.Add( x => x.Value, value ) );

        // validate
        Assert.Null( value );
        Assert.Equal( string.Empty, comp.Find( "input" ).GetAttribute( "value" ) );

        // test
        await comp.Find( "input" ).ChangeAsync( new ChangeEventArgs { Value = "02:05" } );

        // validate
        Assert.Equal( new TimeSpan( 2, 5, 0 ), value );
        Assert.Equal( "02:05", comp.Find( "input" ).GetAttribute( "value" ) );

        // test
        comp.Render( parameters => parameters.Add( x => x.Value, (TimeSpan?)null ) );

        // validate
        Assert.Equal( string.Empty, comp.Find( "input" ).GetAttribute( "value" ) );
    }
}