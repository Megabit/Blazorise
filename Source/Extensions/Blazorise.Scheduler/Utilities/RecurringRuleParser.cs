#region Using Directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
#endregion

namespace Blazorise.Scheduler.Utilities;

/// <summary>
/// Parses a recurrence rule (RRULE) string into a <see cref="SchedulerRecurrenceRule"/>.
/// </summary>
public static class RecurringRuleParser
{
    /// <summary>
    /// Parses a recurrence rule string into a SchedulerRecurrenceRule object.
    /// </summary>
    /// <param name="rrule">The string containing the recurrence rule in a specific format.</param>
    /// <returns>Returns a SchedulerRecurrenceRule object populated with parsed values.</returns>
    /// <exception cref="NotSupportedException">Thrown when an unsupported frequency or BYDAY value is encountered.</exception>
    public static SchedulerRecurrenceRule Parse( string rrule )
    {
        var rule = new SchedulerRecurrenceRule();

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
                        _ => SchedulerRecurrencePattern.Never,
                    };
                    break;

                case "INTERVAL":
                    if ( int.TryParse( value, out var interval ) )
                        rule.Interval = interval;
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

                case "BYMONTHDAY":
                    {
                        var days = value.Split( ',', StringSplitOptions.RemoveEmptyEntries );

                        foreach ( var dayStr in days )
                        {
                            if ( int.TryParse( dayStr, out var day ) )
                            {
                                rule.ByMonthDay ??= new List<int>();
                                rule.ByMonthDay.Add( day );
                            }
                            else
                            {
                                throw new FormatException( $"Invalid BYMONTHDAY value: {dayStr}" );
                            }
                        }
                    }
                    break;

                case "BYDAY":
                    {
                        var entries = value.Split( ',', StringSplitOptions.RemoveEmptyEntries );
                        foreach ( var entry in entries )
                        {
                            var match = Regex.Match( entry, @"^(?<prefix>[+-]?\d)?(?<day>[A-Z]{2})$" );
                            if ( !match.Success )
                                throw new FormatException( $"Invalid BYDAY value: {entry}" );

                            var prefix = match.Groups["prefix"].Value;
                            var dayStr = match.Groups["day"].Value;

                            var day = dayStr switch
                            {
                                "MO" => DayOfWeek.Monday,
                                "TU" => DayOfWeek.Tuesday,
                                "WE" => DayOfWeek.Wednesday,
                                "TH" => DayOfWeek.Thursday,
                                "FR" => DayOfWeek.Friday,
                                "SA" => DayOfWeek.Saturday,
                                "SU" => DayOfWeek.Sunday,
                                _ => throw new NotSupportedException( $"Unsupported day: {dayStr}" )
                            };

                            if ( !string.IsNullOrEmpty( prefix ) )
                            {
                                // Monthly with ordinal weekday: e.g., 1MO or -1FR
                                if ( rule.Pattern == SchedulerRecurrencePattern.Monthly || rule.Pattern == SchedulerRecurrencePattern.Yearly )
                                {
                                    rule.ByWeek = (SchedulerWeek)int.Parse( prefix );
                                    rule.ByWeekDay = day;
                                }
                            }
                            else
                            {
                                // Weekly pattern: collect multiple days
                                rule.ByDay ??= new List<DayOfWeek>();
                                rule.ByDay.Add( day );
                            }
                        }
                    }
                    break;

                case "BYMONTH":
                    {
                        // Process BYMONTH parameter for yearly recurrence
                        if ( int.TryParse( value, out var month ) && month >= 1 && month <= 12 )
                        {
                            rule.ByMonth = (SchedulerMonth)month;
                        }
                        else
                        {
                            throw new FormatException( $"Invalid BYMONTH value: {value}. Must be between 1 and 12." );
                        }
                    }
                    break;
            }
        }

        return rule;
    }
}