﻿using System;
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
            var defDate = new DateTime();
            var dateOpen = "<input";
            var dateClose = "</input>";
            var dateType = @"type=""date""";
            var dateOutput = @"<span id=""date-event-initially-undefined-result"">" + defDate.ToString() + "</span>";
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
            var defDate = new DateTimeOffset();
            var dateOpen = "<input";
            var dateClose = "</input>";
            var dateType = @"type=""date""";
            var dateOutput = @"<span id=""date-offset-event-initially-undefined-result"">" + defDate.ToString().Replace("+", "&#x2B;" ) + "</span>";
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
            var dateOutput = @"<span id=""date-event-initially-undefined-result"">" + new DateTime( 1970, 5, 3 ).ToString() + "</span>";
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
            var dateOutput = @"<span id=""nullable-date-event-initially-null-result"">" + new DateTime( 1970, 5, 3 ).ToString()  + "</span>";
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
            var offset = new DateTimeOffset( new DateTime( 2020, 4, 13 ), new TimeSpan( 0, 0, 0 ) );
            var dateOutput = @"<span id=""date-offset-event-initially-undefined-result"">" + offset.ToString().Replace( "+", "&#x2B;" ) + "</span>";
            var comp = RenderComponent<DateEditComponent>();

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
            var offset = new DateTimeOffset( new DateTime( 2020, 4, 13 ), new TimeSpan( 0, 0, 0 ) );
            var dateOutput = @"<span id=""nullable-date-offset-event-initially-null-result"">" + offset.ToString().Replace( "+", "&#x2B;" ) + "</span>";
            var comp = RenderComponent<DateEditComponent>();

            // test
            comp.Instance.NullableOffsetValue = offset;
            comp.Render();

            // validate
            Assert.Contains( dateOutput, comp.Markup );
        }
    }
}
