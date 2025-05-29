using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom.Events;
using Blazorise.Scheduler.Utilities;
using Xunit;

namespace Blazorise.Tests.Scheduler.Utils;

public class SchedulerCalculatorTests
{
    [Fact]
    public void Daily_For_10_Occurrences()
    {
        var recurrenceRule = RecurringRuleParser.Parse( "RRULE:FREQ=DAILY;COUNT=10" );

        var result = RecurringRuleCalculators.GetDailyRecurringDates(
            new DateTime( 1997, 9, 2 ),
            new DateTime( 1997, 9, 1 ),
            new DateTime( 1997, 9, 15 ),
            recurrenceRule ).ToArray();

        Assert.InRange( result.Length, 1, 10 );
        Assert.True( result.All( x => x >= new DateTime( 1997, 9, 2 ) && x <= new DateTime( 1997, 9, 11 ) ) );
    }

    [Fact]
    public void Daily_Until_December_24_1997()
    {
        var recurrenceRule = RecurringRuleParser.Parse( "FREQ=DAILY;UNTIL=19971224T000000Z" );

        var result = RecurringRuleCalculators.GetDailyRecurringDates(
            new DateTime( 1997, 9, 2 ),
            DateTime.MinValue,
            DateTime.MaxValue,
            recurrenceRule ).ToArray();

        Assert.InRange( result.Length, 1, 114 );
        Assert.True( result.All( x => x >= new DateTime( 1997, 9, 2 ) && x <= new DateTime( 1997, 12, 23 ) ) );
    }

    [Fact]
    public void Daily_Every_Other_Day_Until_September_11_1997()
    {
        var recurrenceRule = RecurringRuleParser.Parse( "FREQ=DAILY;INTERVAL=2;UNTIL=19970911T000000Z" );

        var result = RecurringRuleCalculators.GetDailyRecurringDates(
            new DateTime( 1997, 9, 2 ),
            DateTime.MinValue,
            DateTime.MaxValue,
            recurrenceRule ).ToArray();

        Assert.Collection( result,
            x => Assert.Equal( new DateTime( 1997, 9, 2 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 4 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 6 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 8 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 10 ), x ) );
    }

    [Fact]
    public void Daily_Every_Third_Day_Until_September_11_1997()
    {
        var recurrenceRule = RecurringRuleParser.Parse( "FREQ=DAILY;INTERVAL=3;UNTIL=19970911T000000Z" );

        var result = RecurringRuleCalculators.GetDailyRecurringDates(
            new DateTime( 1997, 9, 2 ),
            DateTime.MinValue,
            DateTime.MaxValue,
            recurrenceRule ).ToArray();

        Assert.Collection( result,
            x => Assert.Equal( new DateTime( 1997, 9, 2 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 5 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 8 ), x ) );
    }

    [Fact]
    public void Daily_Every_10_Days_5_Occurrences()
    {
        var recurrenceRule = RecurringRuleParser.Parse( "FREQ=DAILY;INTERVAL=10;COUNT=5" );

        var result = RecurringRuleCalculators.GetDailyRecurringDates(
            new DateTime( 1997, 9, 2 ),
            DateTime.MinValue,
            DateTime.MaxValue,
            recurrenceRule ).ToArray();

        Assert.Collection( result,
            x => Assert.Equal( new DateTime( 1997, 9, 2 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 12 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 22 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 2 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 12 ), x ) );
    }

    [Fact]
    public void Weekly_For_10_Occurrences()
    {
        var recurrenceRule = RecurringRuleParser.Parse( "FREQ=WEEKLY;COUNT=10" );

        var result = RecurringRuleCalculators.GetWeeklyRecurringDates(
            new DateTime( 1997, 9, 2 ),
            new DateTime( 1997, 9, 2 ),
            DateTime.MaxValue,
            DayOfWeek.Sunday,
            recurrenceRule ).ToArray();

        Assert.Collection( result,
            x => Assert.Equal( new DateTime( 1997, 9, 2 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 9 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 16 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 23 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 30 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 7 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 14 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 21 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 28 ), x ),
            x => Assert.Equal( new DateTime( 1997, 11, 4 ), x ) );
    }

    [Fact]
    public void Weekly_Until_December_24_1997()
    {
        var recurrenceRule = RecurringRuleParser.Parse( "FREQ=WEEKLY;UNTIL=19971224T000000Z" );

        var result = RecurringRuleCalculators.GetWeeklyRecurringDates(
            new DateTime( 1997, 9, 2 ),
            new DateTime( 1997, 9, 2 ),
            DateTime.MaxValue,
            DayOfWeek.Sunday,
            recurrenceRule ).ToArray();

        Assert.Collection( result,
            x => Assert.Equal( new DateTime( 1997, 9, 2 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 9 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 16 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 23 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 30 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 7 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 14 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 21 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 28 ), x ),
            x => Assert.Equal( new DateTime( 1997, 11, 4 ), x ),
            x => Assert.Equal( new DateTime( 1997, 11, 11 ), x ),
            x => Assert.Equal( new DateTime( 1997, 11, 18 ), x ),
            x => Assert.Equal( new DateTime( 1997, 11, 25 ), x ),
            x => Assert.Equal( new DateTime( 1997, 12, 2 ), x ),
            x => Assert.Equal( new DateTime( 1997, 12, 9 ), x ),
            x => Assert.Equal( new DateTime( 1997, 12, 16 ), x ),
            x => Assert.Equal( new DateTime( 1997, 12, 23 ), x ) );
    }

    [Fact]
    public void Weekly_Every_Other_Week_Forever()
    {
        var recurrenceRule = RecurringRuleParser.Parse( "FREQ=WEEKLY;INTERVAL=2;" );

        var result = RecurringRuleCalculators.GetWeeklyRecurringDates(
            new DateTime( 1997, 9, 2 ),
            new DateTime( 1997, 9, 2 ),
            DateTime.MaxValue,
            DayOfWeek.Sunday,
            recurrenceRule ).ToArray();

        Assert.Equal( 99, result.Length );

        Assert.Collection( result.Take( 9 ),
            x => Assert.Equal( new DateTime( 1997, 9, 2 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 16 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 30 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 14 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 28 ), x ),
            x => Assert.Equal( new DateTime( 1997, 11, 11 ), x ),
            x => Assert.Equal( new DateTime( 1997, 11, 25 ), x ),
            x => Assert.Equal( new DateTime( 1997, 12, 9 ), x ),
            x => Assert.Equal( new DateTime( 1997, 12, 23 ), x ) );
    }

    [Fact]
    public void Weekly_on_Tuesday_and_Thursday_for_five_weeks()
    {
        var recurrenceRule = RecurringRuleParser.Parse( "FREQ=WEEKLY;UNTIL=19971007T000000Z;BYDAY=TU,TH" );

        var result = RecurringRuleCalculators.GetWeeklyRecurringDates(
            new DateTime( 1997, 9, 2 ),
            new DateTime( 1997, 9, 2 ),
            DateTime.MaxValue,
            DayOfWeek.Sunday,
            recurrenceRule ).ToArray();

        Assert.Collection( result,
            x => Assert.Equal( new DateTime( 1997, 9, 2 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 4 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 9 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 11 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 16 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 18 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 23 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 25 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 30 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 2 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 7 ), x ) );
    }

    [Fact]
    public void Every_other_week_on_Tuesday_and_Thursday_for_8_occurrences()
    {
        var recurrenceRule = RecurringRuleParser.Parse( "FREQ=WEEKLY;INTERVAL=2;COUNT=8;BYDAY=TU,TH" );

        var result = RecurringRuleCalculators.GetWeeklyRecurringDates(
            new DateTime( 1997, 9, 2 ),
            new DateTime( 1997, 9, 2 ),
            DateTime.MaxValue,
            DayOfWeek.Sunday,
            recurrenceRule ).ToArray();

        Assert.Collection( result,
            x => Assert.Equal( new DateTime( 1997, 9, 2 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 4 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 16 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 18 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 30 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 2 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 14 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 16 ), x ) );
    }

    [Fact]
    public void Monthly_on_the_first_Friday_for_10_occurrences()
    {
        var recurrenceRule = RecurringRuleParser.Parse( "FREQ=MONTHLY;COUNT=10;BYDAY=1FR" );

        var result = RecurringRuleCalculators.GetMonthlyRecurringDates(
            new DateTime( 1997, 9, 2 ),
            new DateTime( 1997, 9, 2 ),
            DateTime.MaxValue,
            DayOfWeek.Sunday,
            recurrenceRule ).ToArray();

        Assert.Collection( result,
            x => Assert.Equal( new DateTime( 1997, 9, 5 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 3 ), x ),
            x => Assert.Equal( new DateTime( 1997, 11, 7 ), x ),
            x => Assert.Equal( new DateTime( 1997, 12, 5 ), x ),
            x => Assert.Equal( new DateTime( 1998, 1, 2 ), x ),
            x => Assert.Equal( new DateTime( 1998, 2, 6 ), x ),
            x => Assert.Equal( new DateTime( 1998, 3, 6 ), x ),
            x => Assert.Equal( new DateTime( 1998, 4, 3 ), x ),
            x => Assert.Equal( new DateTime( 1998, 5, 1 ), x ),
            x => Assert.Equal( new DateTime( 1998, 6, 5 ), x ) );
    }

    [Fact]
    public void Monthly_on_the_first_Friday_until_December_24_1997()
    {
        var recurrenceRule = RecurringRuleParser.Parse( "FREQ=MONTHLY;UNTIL=19971224T000000Z;BYDAY=1FR" );

        var result = RecurringRuleCalculators.GetMonthlyRecurringDates(
            new DateTime( 1997, 9, 2 ),
            new DateTime( 1997, 9, 2 ),
            DateTime.MaxValue,
            DayOfWeek.Sunday,
            recurrenceRule ).ToArray();

        Assert.Collection( result,
            x => Assert.Equal( new DateTime( 1997, 9, 5 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 3 ), x ),
            x => Assert.Equal( new DateTime( 1997, 11, 7 ), x ),
            x => Assert.Equal( new DateTime( 1997, 12, 5 ), x ) );
    }

    [Fact]
    public void Every_other_month_on_the_first_Sunday_of_the_month_for_4_occurrences()
    {
        var recurrenceRule = RecurringRuleParser.Parse( "FREQ=MONTHLY;INTERVAL=2;COUNT=4;BYDAY=1SU" );

        var result = RecurringRuleCalculators.GetMonthlyRecurringDates(
            new DateTime( 1997, 9, 2 ),
            new DateTime( 1997, 9, 2 ),
            DateTime.MaxValue,
            DayOfWeek.Sunday,
            recurrenceRule ).ToArray();

        Assert.Collection( result,
            x => Assert.Equal( new DateTime( 1997, 9, 7 ), x ),
            x => Assert.Equal( new DateTime( 1997, 11, 2 ), x ),
            x => Assert.Equal( new DateTime( 1998, 1, 4 ), x ),
            x => Assert.Equal( new DateTime( 1998, 3, 1 ), x ) );
    }

    [Fact]
    public void Monthly_on_the_2nd_and_15th_of_the_month_for_10_occurrences()
    {
        var recurrenceRule = RecurringRuleParser.Parse( "FREQ=MONTHLY;COUNT=10;BYMONTHDAY=2,15" );

        var result = RecurringRuleCalculators.GetMonthlyRecurringDates(
            new DateTime( 1997, 9, 2 ),
            new DateTime( 1997, 9, 2 ),
            DateTime.MaxValue,
            DayOfWeek.Sunday,
            recurrenceRule ).ToArray();

        Assert.Collection( result,
            x => Assert.Equal( new DateTime( 1997, 9, 2 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 15 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 2 ), x ),
            x => Assert.Equal( new DateTime( 1997, 10, 15 ), x ),
            x => Assert.Equal( new DateTime( 1997, 11, 2 ), x ),
            x => Assert.Equal( new DateTime( 1997, 11, 15 ), x ),
            x => Assert.Equal( new DateTime( 1997, 12, 2 ), x ),
            x => Assert.Equal( new DateTime( 1997, 12, 15 ), x ),
            x => Assert.Equal( new DateTime( 1998, 1, 2 ), x ),
            x => Assert.Equal( new DateTime( 1998, 1, 15 ), x ) );
    }

    [Fact]
    public void Every_Tuesday_every_other_month()
    {
        var recurrenceRule = RecurringRuleParser.Parse( "FREQ=MONTHLY;INTERVAL=2;COUNT=13;BYDAY=TU" );

        var result = RecurringRuleCalculators.GetMonthlyRecurringDates(
            new DateTime( 1997, 9, 2 ),
            new DateTime( 1997, 9, 2 ),
            DateTime.MaxValue,
            DayOfWeek.Sunday,
            recurrenceRule ).ToArray();

        Assert.Collection( result,
            x => Assert.Equal( new DateTime( 1997, 9, 2 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 9 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 16 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 23 ), x ),
            x => Assert.Equal( new DateTime( 1997, 9, 30 ), x ),
            x => Assert.Equal( new DateTime( 1997, 11, 4 ), x ),
            x => Assert.Equal( new DateTime( 1997, 11, 11 ), x ),
            x => Assert.Equal( new DateTime( 1997, 11, 18 ), x ),
            x => Assert.Equal( new DateTime( 1997, 11, 25 ), x ),
            x => Assert.Equal( new DateTime( 1998, 1, 6 ), x ),
            x => Assert.Equal( new DateTime( 1998, 1, 13 ), x ),
            x => Assert.Equal( new DateTime( 1998, 1, 20 ), x ),
            x => Assert.Equal( new DateTime( 1998, 1, 27 ), x ) );
    }
}
