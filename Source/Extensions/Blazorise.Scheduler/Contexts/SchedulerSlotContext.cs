using System;

namespace Blazorise.Scheduler;

/// <summary>
/// Represents the context for a scheduler slot, exposing the slot time range and styling metadata.
/// </summary>
public class SchedulerSlotContext
{
    /// <summary>
    /// A default constructor.
    /// </summary>
    /// <param name="start">The start date and time of the slot.</param>
    /// <param name="end">The end date and time of the slot.</param>
    /// <param name="section">The scheduler section that owns the slot.</param>
    /// <param name="styling">The mutable styling object for the slot.</param>
    /// <param name="isHovered">Indicates whether the slot is currently hovered.</param>
    /// <param name="isDraggingOver">Indicates whether the slot is currently a drag target.</param>
    /// <param name="isLastSlot">Indicates whether the slot is the last slot in the rendered cell.</param>
    internal SchedulerSlotContext( DateTime start, DateTime end, SchedulerSection section, SchedulerSlotStyling styling, bool isHovered, bool isDraggingOver, bool isLastSlot )
    {
        Start = start;
        End = end;
        Section = section;
        Styling = styling;
        IsHovered = isHovered;
        IsDraggingOver = isDraggingOver;
        IsLastSlot = isLastSlot;
    }

    /// <summary>
    /// Gets the slot start date and time.
    /// </summary>
    public DateTime Start { get; }

    /// <summary>
    /// Gets the slot end date and time.
    /// </summary>
    public DateTime End { get; }

    /// <summary>
    /// Gets the scheduler section that owns the slot.
    /// </summary>
    public SchedulerSection Section { get; }

    /// <summary>
    /// Gets the mutable styling object for the current slot.
    /// </summary>
    public SchedulerSlotStyling Styling { get; }

    /// <summary>
    /// Indicates whether the slot is currently hovered.
    /// </summary>
    public bool IsHovered { get; }

    /// <summary>
    /// Indicates whether the slot is currently a drag target.
    /// </summary>
    public bool IsDraggingOver { get; }

    /// <summary>
    /// Indicates whether the slot is the last slot in the rendered cell.
    /// </summary>
    public bool IsLastSlot { get; }
}