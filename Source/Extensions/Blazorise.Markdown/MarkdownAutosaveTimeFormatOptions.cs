namespace Blazorise.Markdown;

/// <summary>
/// Intl.DateTimeFormat options
/// </summary>
public class MarkdownAutosaveTimeFormatOptions
{
    /// <summary>Locale format used for the weekday name.</summary>
    public string Weekday { get; set; }

    /// <summary>Locale format used for the calendar era.</summary>
    public string Era { get; set; }

    /// <summary>Locale format used for the year.</summary>
    public string Year { get; set; }

    /// <summary>Locale format used for the month.</summary>
    public string Month { get; set; }

    /// <summary>Locale format used for the day of the month.</summary>
    public string Day { get; set; }

    /// <summary>Locale format used for the hour.</summary>
    public string Hour { get; set; }

    /// <summary>Locale format used for minutes.</summary>
    public string Minute { get; set; }

    /// <summary>Locale format used for seconds.</summary>
    public string Second { get; set; }

    /// <summary>Locale format used for the time-zone label.</summary>
    public string TimeZoneName { get; set; }
}