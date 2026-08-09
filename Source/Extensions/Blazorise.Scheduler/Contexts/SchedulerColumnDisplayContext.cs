namespace Blazorise.Scheduler;

/// <summary>
/// Provides contextual information for rendering a scheduler column display template.
/// </summary>
/// <typeparam name="TItem">The scheduler item type.</typeparam>
/// <typeparam name="TValue">The scheduler column value type.</typeparam>
public class SchedulerColumnDisplayContext<TItem, TValue>
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SchedulerColumnDisplayContext{TItem, TValue}"/> class.
    /// </summary>
    /// <param name="item">The item being rendered.</param>
    /// <param name="column">The column being rendered.</param>
    /// <param name="value">The current column value.</param>
    /// <param name="isRecurring">Indicates whether the item is recurring.</param>
    internal SchedulerColumnDisplayContext( TItem item, SchedulerColumn<TItem, TValue> column, TValue value, bool isRecurring )
    {
        Item = item;
        Column = column;
        Value = value;
        IsRecurring = isRecurring;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the item being rendered.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Gets the column being rendered.
    /// </summary>
    public SchedulerColumn<TItem, TValue> Column { get; }

    /// <summary>
    /// Gets the current column value.
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    /// Indicates whether the item is recurring.
    /// </summary>
    public bool IsRecurring { get; }

    #endregion
}