#region Using Directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#endregion

namespace Blazorise.Scheduler.Utilities;

/// <summary>
/// Parses a recurrence rule (RRULE) string into a <see cref="SchedulerRecurrenceRule"/>.
/// </summary>
public static class RRuleParser
{
    private static readonly Dictionary<string, DayOfWeek> DayMap = new()
    {
        { "MO", DayOfWeek.Monday },
        { "TU", DayOfWeek.Tuesday },
        { "WE", DayOfWeek.Wednesday },
        { "TH", DayOfWeek.Thursday },
        { "FR", DayOfWeek.Friday },
        { "SA", DayOfWeek.Saturday },
        { "SU", DayOfWeek.Sunday },
    };

    /// <summary>
    /// Parses a recurrence rule string into a SchedulerRecurrenceRule object.
    /// </summary>
    /// <param name="rrule">The string containing the recurrence rule in a specific format.</param>
    /// <returns>Returns a SchedulerRecurrenceRule object populated with parsed values.</returns>
    /// <exception cref="NotSupportedException">Thrown when an unsupported frequency or BYDAY value is encountered.</exception>
    public static SchedulerRecurrenceRule Parse( string rrule )
    {
        var rule = new SchedulerRecurrenceRule
        {
            Interval = 1,
            ByDay = new List<DayOfWeek>()
        };

        var parts = rrule.Split( ';', StringSplitOptions.RemoveEmptyEntries );

        foreach ( var part in parts )
        {
            var kv = part.Split( '=', 2 );

            if ( kv.Length != 2 )
                continue;

            var key = kv[0].ToUpperInvariant();
            var value = kv[1].ToUpperInvariant();

            switch ( key )
            {
                case "FREQ":
                    rule.Pattern = value switch
                    {
                        "DAILY" => SchedulerRecurrencePattern.Daily,
                        "WEEKLY" => SchedulerRecurrencePattern.Weekly,
                        "MONTHLY" => SchedulerRecurrencePattern.Monthly,
                        "YEARLY" => SchedulerRecurrencePattern.Yearly,
                        _ => throw new NotSupportedException( $"Unsupported FREQ: {value}" )
                    };
                    break;

                case "INTERVAL":
                    if ( int.TryParse( value, out var interval ) )
                        rule.Interval = interval;
                    break;

                case "BYDAY":
                    rule.ByDay = value.Split( ',' ).Select( day =>
                    {
                        if ( DayMap.TryGetValue( day, out var dow ) )
                            return dow;
                        throw new NotSupportedException( $"Unsupported BYDAY value: {day}" );
                    } ).ToList();
                    break;

                case "UNTIL":
                    if ( DateTime.TryParseExact(
                            value,
                            new[] { "yyyyMMdd'T'HHmmss'Z'", "yyyyMMdd" },
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                            out var until ) )
                    {
                        rule.EndDate = until;
                    }
                    break;

                case "COUNT":
                    if ( int.TryParse( value, out var count ) )
                        rule.Count = count;
                    break;
            }
        }

        return rule;
    }
}