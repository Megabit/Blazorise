namespace Blazorise.Gantt;

/// <summary>
/// Defines the transaction lifecycle state.
/// </summary>
public enum GanttTransactionState
{
    /// <summary>
    /// Transaction is created but not started.
    /// </summary>
    Pending,

    /// <summary>
    /// Transaction is currently running.
    /// </summary>
    InProgress,

    /// <summary>
    /// Transaction was successfully committed.
    /// </summary>
    Committed,

    /// <summary>
    /// Transaction was rolled back.
    /// </summary>
    RolledBack,
}