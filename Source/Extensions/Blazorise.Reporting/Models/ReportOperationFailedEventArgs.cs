#region Using directives
using System;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Provides information about a failed report operation.
/// </summary>
public sealed class ReportOperationFailedEventArgs : EventArgs
{
    #region Constructors

    /// <summary>
    /// Initializes a new report operation failure.
    /// </summary>
    /// <param name="operation">The operation that failed.</param>
    /// <param name="exception">The failure.</param>
    public ReportOperationFailedEventArgs( string operation, Exception exception )
    {
        Operation = operation;
        Exception = exception;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the operation that failed.
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Gets the failure.
    /// </summary>
    public Exception Exception { get; }

    #endregion
}