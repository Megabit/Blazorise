﻿using System;
using System.Threading.Tasks;
using Blazorise.Tests.Mocks;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Blazorise.Tests
{
    public class DateEditTest
    {
        [Fact]
        public void SetDate_WithDateTime()
        {
            // setup
            var edit = new DateEdit<DateTime>();

            // test
            edit.Date = new DateTime( 2020, 4, 12 );

            // validate
            Assert.Equal( 2020, edit.Date.Year );
            Assert.Equal( 4, edit.Date.Month );
            Assert.Equal( 12, edit.Date.Day );
        }

        [Fact]
        public void SetDate_WithDateTimeOffset()
        {
            // setup
            var edit = new DateEdit<DateTimeOffset>();

            // test
            edit.Date = new DateTimeOffset( new DateTime( 2020, 4, 12 ), new TimeSpan( 0, 0, 0 ) );

            // validate
            Assert.Equal( 2020, edit.Date.Year );
            Assert.Equal( 4, edit.Date.Month );
            Assert.Equal( 12, edit.Date.Day );
        }

        [Fact]
        public void SetDate_WithNull()
        {
            // setup
            var edit = new DateEdit<DateTime?>();

            // test
            edit.Date = null;

            // validate
            Assert.Null( edit.Date );
        }

        [Fact]
        public void SetDate_WithInvalidType()
        {
            // setup
            var edit = new MockDateEdit<int>();

            // test
            edit.Date = 100;

            // validate
            Exception ex = Assert.Throws<InvalidOperationException>( () => edit.TextValue );
        }

        [Fact]
        public async Task OnClickedTest()
        {
            // setup
            var edit = new MockDateEdit<DateTime>();
            var expected = edit.ElementId;

            // test
            await edit.Click();

            // validate
            Assert.Equal( expected, edit.ClickedId );
        }

        [Fact]
        public async Task ParseValueFromStringAsync_ValidDateString()
        {
            // setup
            var edit = new MockDateEdit<DateTime>();
            var expected = new DateTime( 2018, 7, 12 );

            // test
            var result = await edit.ParseValueAsync( expected.ToString() );

            // validate
            Assert.True( result.Success );
            Assert.Equal( expected, result.ParsedValue );
        }

        [Fact]
        public async Task ParseValueFromStringAsync_InvalidDateString()
        {
            // setup
            var edit = new MockDateEdit<DateTime>();

            // test
            var result = await edit.ParseValueAsync( "3/12/2020-invalid" );

            // validate
            Assert.False( result.Success );
            Assert.Equal( default, result.ParsedValue );
        }

        [Fact]
        public void WithParentValidation()
        {
            // setup
            var validation = new Validation();
            System.Linq.Expressions.Expression<Func<DateTime>> expr = () => DateTime.Today;

            // test
            var edit = new MockDateEdit<DateTime>( validation, null );

            // validate
            Assert.NotNull( edit );
        }

        [Fact]
        public void ChangeValue_DateTime()
        {
            // setup
            var edit = new MockDateEdit<DateTime>();
            var expected = new DateTime( 2007, 1, 5 );

            // test
            edit.OnChange( new ChangeEventArgs { Value = expected } );

            // validate
            Assert.Equal( expected, edit.Date );
        }

        [Fact]
        public void ChangeValue_DateTime_NullValue()
        {
            // setup
            var edit = new MockDateEdit<DateTime?>();

            // test
            edit.OnChange( new ChangeEventArgs() );

            // validate
            Assert.Null( edit.Date );
        }

        [Fact]
        public void ChangeValue_DateTime_NullEventArgs()
        {
            // setup
            var edit = new MockDateEdit<DateTime?>();

            // test
            edit.OnChange( null );

            // validate
            Assert.Null( edit.Date );
        }

        [Fact]
        public void ChangeValue_DateTimeOffset()
        {
            // setup
            var edit = new MockDateEdit<DateTimeOffset>();
            var expected = new DateTime( 2007, 1, 5 );

            // test
            edit.OnChange( new ChangeEventArgs { Value = expected } );

            // validate
            Assert.Equal( expected, edit.Date );
        }

        [Fact]
        public void ChangeValue_DateTimeOffset_NullValue()
        {
            // setup
            var edit = new MockDateEdit<DateTimeOffset?>();

            // test
            edit.OnChange( new ChangeEventArgs() );

            // validate
            Assert.Null( edit.Date );
        }

        [Fact]
        public void ChangeValue_DateTimeOffset_NullEventArgs()
        {
            // setup
            var edit = new MockDateEdit<DateTimeOffset?>();

            // test
            edit.OnChange( null );

            // validate
            Assert.Null( edit.Date );
        }

        [Fact]
        public void MinMaxRange_Within()
        {
            // setup
            var edit = new DateEdit<DateTime>();
            var date = new DateTime( 2020, 3, 15 );

            // test
            edit.Min = new DateTime( 2020, 3, 1 );
            edit.Max = new DateTime( 2020, 3, 30 );
            edit.Date = date;

            // validate
            Assert.Equal( date, edit.Date );
        }

        /* todo: turn tests back on after bug fixed.
         * bug: setting date outside of the min-max date range doesn't skip the set or throw an error.
         * 
        [Fact]
        public void MinMaxRange_After()
        {
            // setup
            var edit = new DateEdit<DateTime?>();
            var date = new DateTime( 2020, 4, 15 );

            // test
            edit.Min = new DateTime( 2020, 3, 1 );
            edit.Max = new DateTime( 2020, 3, 30 );
            edit.Date = date;

            // validate
            Assert.Null( edit.Date );
        }

        [Fact]
        public void MinMaxRange_Before()
        {
            // setup
            var edit = new DateEdit<DateTime?>();
            var date = new DateTime( 2018, 3, 15 );

            // test
            edit.Min = new DateTime( 2020, 3, 1 );
            edit.Max = new DateTime( 2020, 3, 30 );
            edit.Date = date;

            // validate
            Assert.Null( edit.Date );
        }
        */
    }
}
