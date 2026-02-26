#region Using directives
using Blazorise.Localization;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Localizers used by the Gantt chart.
/// </summary>
public class GanttLocalizers
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
    /// Localizer for the "Task" label.
    /// </summary>
    public TextLocalizerHandler TaskLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Title" field label.
    /// </summary>
    public TextLocalizerHandler TitleLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Description" field label.
    /// </summary>
    public TextLocalizerHandler DescriptionLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Start" field label.
    /// </summary>
    public TextLocalizerHandler StartLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "End" field label.
    /// </summary>
    public TextLocalizerHandler EndLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Duration" field label.
    /// </summary>
    public TextLocalizerHandler DurationLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Save" action label.
    /// </summary>
    public TextLocalizerHandler SaveLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Cancel" action label.
    /// </summary>
    public TextLocalizerHandler CancelLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Delete" action label.
    /// </summary>
    public TextLocalizerHandler DeleteLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Add Task" action label.
    /// </summary>
    public TextLocalizerHandler AddTaskLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Add Child" action label.
    /// </summary>
    public TextLocalizerHandler AddChildLocalizer { get; set; }

    /// <summary>
    /// Localizer for the search placeholder text.
    /// </summary>
    public TextLocalizerHandler SearchLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Columns" label.
    /// </summary>
    public TextLocalizerHandler ColumnsLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Day" view label.
    /// </summary>
    public TextLocalizerHandler DayLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Week" view label.
    /// </summary>
    public TextLocalizerHandler WeekLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Month" view label.
    /// </summary>
    public TextLocalizerHandler MonthLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Year" view label.
    /// </summary>
    public TextLocalizerHandler YearLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Previous" navigation label.
    /// </summary>
    public TextLocalizerHandler PreviousLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Next" navigation label.
    /// </summary>
    public TextLocalizerHandler NextLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Today" navigation label.
    /// </summary>
    public TextLocalizerHandler TodayLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "Title is required." validation message.
    /// </summary>
    public TextLocalizerHandler TitleRequiredLocalizer { get; set; }

    /// <summary>
    /// Localizer for the "End must be greater than Start." validation message.
    /// </summary>
    public TextLocalizerHandler EndBeforeStartLocalizer { get; set; }

    /// <summary>
    /// Localizer for the task deletion confirmation text.
    /// </summary>
    public TextLocalizerHandler DeleteTaskConfirmationLocalizer { get; set; }

    /// <summary>
    /// Localizer for the empty-state text when no tasks are visible.
    /// </summary>
    public TextLocalizerHandler NoTasksToDisplayLocalizer { get; set; }
}