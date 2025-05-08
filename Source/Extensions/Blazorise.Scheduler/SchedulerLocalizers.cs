#region Using Directives
using Blazorise.Localization;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Provides a set of localization handlers for common labels and messages used throughout the <see cref="Scheduler{TItem}"/> component.
/// </summary>
public class SchedulerLocalizers
{
    /// <summary>
    /// Localizer for the "New" label.
    /// </summary>
    public TextLocalizerHandler NewLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Edit" label.
    /// </summary>
    public TextLocalizerHandler EditLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Title" field label.
    /// </summary>
    public TextLocalizerHandler TitleLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Start" field label.
    /// </summary>
    public TextLocalizerHandler StartLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "End" field label.
    /// </summary>
    public TextLocalizerHandler EndLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "All Day" label.
    /// </summary>
    public TextLocalizerHandler AllDayLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Description" field label.
    /// </summary>
    public TextLocalizerHandler DescriptionLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Delete" action label.
    /// </summary>
    public TextLocalizerHandler DeleteLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Cancel" action label.
    /// </summary>
    public TextLocalizerHandler CancelLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Save" action label.
    /// </summary>
    public TextLocalizerHandler SaveLocalizer { get; set; }

    /// <summary>
    /// Localizer for the item deletion confirmation dialog text.
    /// </summary>
    public TextLocalizerHandler ItemDeleteConfirmationLocalizer { get; set; }

    /// <summary>
    /// Localizer for the occurrence deletion confirmation dialog text.
    /// </summary>
    public TextLocalizerHandler OccurrenceDeleteConfirmationLocalizer { get; set; }

    /// <summary>
    /// Localizer for the series deletion confirmation dialog text.
    /// </summary>
    public TextLocalizerHandler SeriesDeleteConfirmationTextLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Delete Series" label.
    /// </summary>
    public TextLocalizerHandler DeleteSeriesLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Delete Occurrence" label.
    /// </summary>
    public TextLocalizerHandler DeleteOccurrenceLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Edit Series" label.
    /// </summary>
    public TextLocalizerHandler EditSeriesLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Edit Occurrence" label.
    /// </summary>
    public TextLocalizerHandler EditOccurrenceLocalizer { get; set; }

    /// <summary>
    /// Localizer for the generic "Appointment" label.
    /// </summary>
    public TextLocalizerHandler AppointmentLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Today" navigation button.
    /// </summary>
    public TextLocalizerHandler TodayLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Day" view label.
    /// </summary>
    public TextLocalizerHandler DayLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Week" view label.
    /// </summary>
    public TextLocalizerHandler WeekLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Work Week" view label.
    /// </summary>
    public TextLocalizerHandler WorkWeekLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Month" view label.
    /// </summary>
    public TextLocalizerHandler MonthLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Year" view label.
    /// </summary>
    public TextLocalizerHandler YearLocalizer { get; set; }

    /// <summary>
    /// Localizer for the word "On" in recurrence phrases.
    /// </summary>
    public TextLocalizerHandler OnLocalizer { get; set; }

    /// <summary>
    /// Localizer for the phrase "Week of month" in recurrence settings.
    /// </summary>
    public TextLocalizerHandler WeekOfMonthLocalizer { get; set; }

    /// <summary>
    /// Localizer for the phrase "Day of week" in recurrence settings.
    /// </summary>
    public TextLocalizerHandler DayOfWeekLocalizer { get; set; }

    /// <summary>
    /// Localizer for the phrase "Month of year" in recurrence settings.
    /// </summary>
    public TextLocalizerHandler MonthOfYearLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Count" field in recurrence rules.
    /// </summary>
    public TextLocalizerHandler CountLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Never" option in recurrence end settings.
    /// </summary>
    public TextLocalizerHandler NeverLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Repeat every" label in recurrence rule UI.
    /// </summary>
    public TextLocalizerHandler RepeatEveryLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "What do you want to do" label in the message dialog.
    /// </summary>
    public TextLocalizerHandler WhatDoYouWantToDoLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Item is a recurring series. What do you want to do" label in the message dialog.
    /// </summary>
    public TextLocalizerHandler RecurringSeriesWhatDoYouWantToDoLocalizer { get; set; }
}
