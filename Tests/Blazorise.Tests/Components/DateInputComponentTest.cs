using System;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Blazorise.Tests.Components;

public class DateInputComponentTest : BunitContext
{
    public DateInputComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseUtilities();
    }

    [Fact]
    public async Task NativeDateInputPreservesEmptyEditingValueUntilCompletedOrReplaced()
    {
        // setup
        DateOnly value = new( 2030, 3, 15 );
        IRenderedComponent<DateInput<DateOnly>> comp = Render<DateInput<DateOnly>>( parameters => parameters
            .Add( x => x.Value, value )
            .Add( x => x.ValueChanged, changedValue => value = changedValue ) );

        // test
        await comp.Find( "input" ).ChangeAsync( new ChangeEventArgs { Value = string.Empty } );
        comp.Render( parameters => parameters.Add( x => x.Value, value ) );

        // validate existing default-value behavior without writing it back into the input
        Assert.Equal( default, value );
        Assert.Equal( string.Empty, comp.Find( "input" ).GetAttribute( "value" ) );

        // test
        await comp.Find( "input" ).ChangeAsync( new ChangeEventArgs { Value = "2030-01-05" } );

        // validate
        Assert.Equal( new DateOnly( 2030, 1, 5 ), value );
        Assert.Equal( "2030-01-05", comp.Find( "input" ).GetAttribute( "value" ) );

        // test
        comp.Render( parameters => parameters.Add( x => x.Value, new DateOnly( 2031, 6, 12 ) ) );

        // validate
        Assert.Equal( "2031-06-12", comp.Find( "input" ).GetAttribute( "value" ) );
    }

    [Fact]
    public async Task NullableNativeDateInputPreservesEmptyEditingValueAcrossRenders()
    {
        // setup
        DateOnly? value = new DateOnly( 2030, 3, 15 );
        IRenderedComponent<DateInput<DateOnly?>> comp = Render<DateInput<DateOnly?>>( parameters => parameters
            .Add( x => x.Value, value )
            .Add( x => x.ValueChanged, changedValue => value = changedValue ) );

        // test
        await comp.Find( "input" ).ChangeAsync( new ChangeEventArgs { Value = string.Empty } );
        comp.Render( parameters => parameters.Add( x => x.Value, value ) );

        // validate
        Assert.Null( value );
        Assert.Equal( string.Empty, comp.Find( "input" ).GetAttribute( "value" ) );

        // test
        await comp.Find( "input" ).ChangeAsync( new ChangeEventArgs { Value = "2030-05-02" } );

        // validate
        Assert.Equal( new DateOnly( 2030, 5, 2 ), value );
        Assert.Equal( "2030-05-02", comp.Find( "input" ).GetAttribute( "value" ) );

        // test
        comp.Render( parameters => parameters.Add( x => x.Value, (DateOnly?)null ) );

        // validate
        Assert.Equal( string.Empty, comp.Find( "input" ).GetAttribute( "value" ) );
    }

    [Fact]
    public async Task NativeEditingValueIsReformattedWhenInputModeChanges()
    {
        // setup
        IRenderedComponent<DateInput<DateTime>> comp = Render<DateInput<DateTime>>( parameters => parameters
            .Add( x => x.Value, new DateTime( 2030, 3, 15 ) ) );

        // test
        await comp.Find( "input" ).ChangeAsync( new ChangeEventArgs { Value = "2030-03-16" } );
        comp.Render( parameters => parameters
            .Add( x => x.Value, comp.Instance.Value )
            .Add( x => x.InputMode, DateInputMode.Month ) );

        // validate
        Assert.Equal( "month", comp.Find( "input" ).GetAttribute( "type" ) );
        Assert.Equal( "2030-03", comp.Find( "input" ).GetAttribute( "value" ) );
        Assert.Equal( new DateTime( 2030, 3, 16 ), comp.Instance.Value );
    }

    [Fact]
    public async Task WeekModeUsesNativeWeekValue()
    {
        // setup
        IRenderedComponent<DateInput<DateTime?>> comp = Render<DateInput<DateTime?>>( parameters => parameters
            .Add( x => x.Value, new DateTime( 2026, 10, 8 ) )
            .Add( x => x.InputMode, DateInputMode.Week ) );

        // validate
        Assert.Equal( "week", comp.Find( "input" ).GetAttribute( "type" ) );
        Assert.Equal( "2026-W41", comp.Find( "input" ).GetAttribute( "value" ) );

        // test
        await comp.Find( "input" ).ChangeAsync( new ChangeEventArgs { Value = "2026-W42" } );

        // validate
        Assert.Equal( new DateTime( 2026, 10, 12 ), comp.Instance.Value );
    }

    [Fact]
    public void RenderDateTimeTest()
    {
        // setup
        var defDate = new DateTime();
        var dateOpen = "<input";
        var dateClose = "</input>";
        var dateType = @"type=""date""";
        var dateOutput = @"<span id=""date-event-initially-undefined-result"">" + defDate.ToString() + "</span>";
        var nullableOutput = @"<span id=""nullable-date-event-initially-null-result""></span>";

        // test
        var comp = Render<DateInputComponent>();

        // validate
        Assert.Contains( dateOpen, comp.Markup );
        Assert.Contains( dateClose, comp.Markup );
        Assert.Contains( dateType, comp.Markup );
        Assert.Contains( dateOutput, comp.Markup );
        Assert.NotNull( comp.Find( "#date-event-initially-undefined" ) );
        Assert.NotNull( comp.Find( "#date-control" ) );
        Assert.NotNull( comp.Find( "#date-event-initially-undefined-result" ) );

        Assert.Contains( nullableOutput, comp.Markup );
        Assert.NotNull( comp.Find( "#nullable-date-event-initially-null" ) );
        Assert.NotNull( comp.Find( "#nullable-date-control" ) );
        Assert.NotNull( comp.Find( "#nullable-date-event-initially-null-result" ) );
    }

    [Fact]
    public void RenderDateOnlyTest()
    {
        // setup
        var defDate = new DateOnly();
        var dateOpen = "<input";
        var dateClose = "</input>";
        var dateType = @"type=""date""";
        var dateOutput = @"<span id=""date-only-event-initially-undefined-result"">" + defDate.ToString() + "</span>";
        var nullableOutput = @"<span id=""nullable-date-only-event-initially-null-result""></span>";

        // test
        var comp = Render<DateInputComponent>();

        // validate
        Assert.Contains( dateOpen, comp.Markup );
        Assert.Contains( dateClose, comp.Markup );
        Assert.Contains( dateType, comp.Markup );
        Assert.Contains( dateOutput, comp.Markup );
        Assert.NotNull( comp.Find( "#date-only-event-initially-undefined" ) );
        Assert.NotNull( comp.Find( "#date-only-control" ) );
        Assert.NotNull( comp.Find( "#date-only-event-initially-undefined-result" ) );

        Assert.Contains( nullableOutput, comp.Markup );
        Assert.NotNull( comp.Find( "#nullable-date-only-event-initially-null" ) );
        Assert.NotNull( comp.Find( "#nullable-date-only-control" ) );
        Assert.NotNull( comp.Find( "#nullable-date-only-event-initially-null-result" ) );
    }

    [Fact]
    public void RenderDateTimeOffsetTest()
    {
        // setup
        var defDate = new DateTimeOffset();
        var dateOpen = "<input";
        var dateClose = "</input>";
        var dateType = @"type=""date""";
        var dateOutput = @"<span id=""date-offset-event-initially-undefined-result"">" + defDate.ToString() + "</span>";
        var nullableOutput = @"<span id=""nullable-date-offset-event-initially-null-result""></span>";

        // test
        var comp = Render<DateInputComponent>();

        // validate
        Assert.Contains( dateOpen, comp.Markup );
        Assert.Contains( dateClose, comp.Markup );
        Assert.Contains( dateType, comp.Markup );
        Assert.Contains( dateOutput, comp.Markup );
        Assert.NotNull( comp.Find( "#date-offset-event-initially-undefined" ) );
        Assert.NotNull( comp.Find( "#date-offset-control" ) );
        Assert.NotNull( comp.Find( "#date-offset-event-initially-undefined-result" ) );

        Assert.Contains( nullableOutput, comp.Markup );
        Assert.NotNull( comp.Find( "#nullable-date-offset-event-initially-null" ) );
        Assert.NotNull( comp.Find( "#nullable-date-offset-control" ) );
        Assert.NotNull( comp.Find( "#nullable-date-offset-event-initially-null-result" ) );
    }

    [Fact]
    public void SetDateTime()
    {
        // setup
        var dateOutput = @"<span id=""date-event-initially-undefined-result"">" + new DateTime( 1970, 5, 3 ).ToString() + "</span>";
        var comp = Render<DateInputComponent>();

        // test
        comp.Instance.DateValue = new( 1970, 5, 3 );
        comp.Render();

        // validate
        Assert.Contains( dateOutput, comp.Markup );
    }

    [Fact]
    public void SetNullableDateTime()
    {
        // setup
        var dateOutput = @"<span id=""nullable-date-event-initially-null-result"">" + new DateTime( 1970, 5, 3 ).ToString() + "</span>";
        var comp = Render<DateInputComponent>();

        // test
        comp.Instance.NullableDateValue = new DateTime( 1970, 5, 3 );
        comp.Render();

        // validate
        Assert.Contains( dateOutput, comp.Markup );
    }

    [Fact]
    public void SetDateOnly()
    {
        // setup
        var dateonly = new DateOnly( 2020, 4, 13 );
        var dateOutput = @"<span id=""date-only-event-initially-undefined-result"">" + dateonly.ToString() + "</span>";
        var comp = Render<DateInputComponent>();

        // test
        comp.Instance.DateOnlyValue = dateonly;
        comp.Render();

        // validate
        Assert.Contains( dateOutput, comp.Markup );
    }

    [Fact]
    public void SetNullableDateOnly()
    {
        // setup
        var dateonly = new DateOnly( 2020, 4, 13 );
        var dateOutput = @"<span id=""nullable-date-only-event-initially-null-result"">" + dateonly.ToString() + "</span>";
        var comp = Render<DateInputComponent>();

        // test
        comp.Instance.NullableDateOnlyValue = dateonly;
        comp.Render();

        // validate
        Assert.Contains( dateOutput, comp.Markup );
    }

    [Fact]
    public void SetDateTimeOffset()
    {
        // setup
        var offset = new DateTimeOffset( new( 2020, 4, 13 ) );
        var dateOutput = @"<span id=""date-offset-event-initially-undefined-result"">" + offset.ToString() + "</span>";
        var comp = Render<DateInputComponent>();

        // test
        comp.Instance.OffsetValue = offset;
        comp.Render();

        // validate
        Assert.Contains( dateOutput, comp.Markup );
    }

    [Fact]
    public void SetNullableDateTimeOffset()
    {
        // setup
        var offset = new DateTimeOffset( new( 2020, 4, 13 ) );
        var dateOutput = @"<span id=""nullable-date-offset-event-initially-null-result"">" + offset.ToString() + "</span>";
        var comp = Render<DateInputComponent>();

        // test
        comp.Instance.NullableOffsetValue = offset;
        comp.Render();

        // validate
        Assert.Contains( dateOutput, comp.Markup );
    }
}