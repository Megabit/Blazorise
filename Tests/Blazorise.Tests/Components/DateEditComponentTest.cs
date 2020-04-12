using System;
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components
{
    public class DateEditComponentTest : ComponentTestFixture
    {
        public DateEditComponentTest()
        {
            BlazoriseConfig.AddBootstrapProviders( Services );
        }

        [Fact]
        public void RenderDateTimeTest()
        {
            // setup
            var dateOpen = "<input";
            var dateClose = "</input>";
            var dateType = @"type=""date""";
            var dateOutput = @"<span id=""date-event-initially-undefined-result"">1/1/0001 12:00:00 AM</span>";
            var nullableOutput = @"<span id=""nullable-date-event-initially-null-result""></span>";

            // test
            var comp = RenderComponent<DateEditComponent>();

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
        public void RenderDateTimeOffsetTest()
        {
            // setup
            var dateOpen = "<input";
            var dateClose = "</input>";
            var dateType = @"type=""date""";
            var dateOutput = @"<span id=""date-offset-event-initially-undefined-result"">1/1/0001 12:00:00 AM &#x2B;00:00</span>";
            var nullableOutput = @"<span id=""nullable-date-offset-event-initially-null-result""></span>";

            // test
            var comp = RenderComponent<DateEditComponent>();

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
            var dateOutput = @"<span id=""date-event-initially-undefined-result"">5/3/1970 12:00:00 AM</span>";
            var comp = RenderComponent<DateEditComponent>();

            // test
            comp.Instance.DateValue = new DateTime( 1970, 5, 3 );
            comp.Render();

            // validate
            Assert.Contains( dateOutput, comp.Markup );
        }

        [Fact]
        public void SetNullableDateTime()
        {
            // setup
            var dateOutput = @"<span id=""nullable-date-event-initially-null-result"">5/3/1970 12:00:00 AM</span>";
            var comp = RenderComponent<DateEditComponent>();

            // test
            comp.Instance.NullableDateValue = new DateTime( 1970, 5, 3 );
            comp.Render();

            // validate
            Assert.Contains( dateOutput, comp.Markup );
        }

        [Fact]
        public void SetDateTimeOffset()
        {
            // setup
            var dateOutput = @"<span id=""date-offset-event-initially-undefined-result"">4/13/2020 12:00:00 AM &#x2B;00:00</span>";
            var comp = RenderComponent<DateEditComponent>();

            // test
            comp.Instance.OffsetValue = new DateTimeOffset( new DateTime( 2020, 4, 13 ), new TimeSpan( 0, 0, 0 ) );
            comp.Render();

            // validate
            Assert.Contains( dateOutput, comp.Markup );
        }

        [Fact]
        public void SetNullableDateTimeOffset()
        {
            // setup
            var dateOutput = @"<span id=""nullable-date-offset-event-initially-null-result"">4/13/2020 12:00:00 AM &#x2B;00:00</span>";
            var comp = RenderComponent<DateEditComponent>();

            // test
            comp.Instance.NullableOffsetValue = new DateTimeOffset( new DateTime( 2020, 4, 13 ), new TimeSpan( 0, 0, 0 ) );
            comp.Render();

            // validate
            Assert.Contains( dateOutput, comp.Markup );
        }
    }
}
