using System;
using System.Threading.Tasks;
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using VerifyXunit;
using Xunit;

namespace Blazorise.Tests.Components
{
    [UsesVerify]
    public class DatePickerComponentTest : TestContext
    {
        public DatePickerComponentTest()
        {
            BlazoriseConfig.AddBootstrapProviders( Services );
        }

        [Fact]
        public Task RenderDateTimeTest()
        {
            // test
            var comp = RenderComponent<DatePickerComponent>();

            // validate
            return Verifier.Verify( comp );
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
        public void SetDateTimeOffset()
        {
            // setup
            var offset = new DateTimeOffset( new( 2020, 4, 13 ) );
            var dateOutput = @"<span id=""date-offset-event-initially-undefined-result"">" + offset.ToString().Replace( "+", "&#x2B;" ) + "</span>";
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
            var dateOutput = @"<span id=""nullable-date-offset-event-initially-null-result"">" + offset.ToString().Replace( "+", "&#x2B;" ) + "</span>";
            var comp = RenderComponent<DatePickerComponent>();

            // test
            comp.Instance.NullableOffsetValue = offset;
            comp.Render();

            // validate
            Assert.Contains( dateOutput, comp.Markup );
        }
    }
}
