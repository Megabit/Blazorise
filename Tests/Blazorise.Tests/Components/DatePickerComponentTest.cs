using System;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;

public class DatePickerComponentTest : TestContext
{
    public DatePickerComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseDatePicker();
    }

    [Fact]
    public void RenderDateTimeTest()
    {
        // setup
        var defDate = new DateTime();
        var dateOpen = "<input";
        var dateClose = "</input>";
        var dateType = @"type=""text""";
        var dateOutput = @"<span id=""date-event-initially-undefined-result"">" + defDate.ToString() + "</span>";
        var nullableOutput = @"<span id=""nullable-date-event-initially-null-result""></span>";

        // test
        var comp = RenderComponent<DatePickerComponent>();

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
        var dateType = @"type=""text""";
        var dateOutput = @"<span id=""date-only-event-initially-undefined-result"">" + defDate.ToString() + "</span>";
        var nullableOutput = @"<span id=""nullable-date-only-event-initially-null-result""></span>";

        // test
        var comp = RenderComponent<DatePickerComponent>();

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
        var dateType = @"type=""text""";
        var dateOutput = @"<span id=""date-offset-event-initially-undefined-result"">" + defDate.ToString() + "</span>";
        var nullableOutput = @"<span id=""nullable-date-offset-event-initially-null-result""></span>";

        // test
        var comp = RenderComponent<DatePickerComponent>();

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
        var comp = RenderComponent<DatePickerComponent>();

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
        var comp = RenderComponent<DatePickerComponent>();

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
        var comp = RenderComponent<DatePickerComponent>();

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
        var comp = RenderComponent<DatePickerComponent>();

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
        var comp = RenderComponent<DatePickerComponent>();

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
        var comp = RenderComponent<DatePickerComponent>();

        // test
        comp.Instance.NullableOffsetValue = offset;
        comp.Render();

        // validate
        Assert.Contains( dateOutput, comp.Markup );
    }
}