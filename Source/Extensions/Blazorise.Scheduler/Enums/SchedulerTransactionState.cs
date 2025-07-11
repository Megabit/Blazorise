namespace Blazorise.Scheduler;

/// <summary>
/// Represents the various states of a scheduler transaction.
/// </summary>
public enum SchedulerTransactionState
{
    /// <summary>
    /// Indicates that the transaction is pending and has not yet been processed.
    /// </summary>
    Pending,

    /// <summary>
    /// Indicates the current status of a process as ongoing.
    /// </summary>
    InProgress,

    /// <summary>
    /// Indicates whether a transaction has been committed. A committed transaction is finalized and cannot be rolled back.
    /// </summary>
    Committed,

    /// <summary>
    /// Indicates that a transaction or operation has been rolled back.
    /// </summary>
    RolledBack
}