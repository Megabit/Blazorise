#region Using directives
using System;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Describes the current progress of a report operation.
/// </summary>
public sealed class ReportProgress
{
    #region Constructors

    /// <summary>
    /// Initializes a new report progress state.
    /// </summary>
    /// <param name="status">The current operation status.</param>
    /// <param name="progress">Known progress in the range from 0 to 1, or <see langword="null"/> for an indeterminate stage.</param>
    /// <param name="completed">The number of completed work items.</param>
    /// <param name="total">The total number of work items.</param>
    public ReportProgress( string status, double? progress = null, int completed = 0, int total = 0 )
    {
        Status = status;
        Progress = progress.HasValue ? Math.Clamp( progress.Value, 0, 1 ) : null;
        Completed = Math.Max( 0, completed );
        Total = Math.Max( 0, total );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current operation status.
    /// </summary>
    public string Status { get; }

    /// <summary>
    /// Gets known progress in the range from 0 to 1, or <see langword="null"/> for an indeterminate stage.
    /// </summary>
    public double? Progress { get; }

    /// <summary>
    /// Gets known progress in the range from 0 to 100, or <see langword="null"/> for an indeterminate stage.
    /// </summary>
    public double? Percentage => Progress * 100d;

    /// <summary>
    /// Gets the number of completed work items.
    /// </summary>
    public int Completed { get; }

    /// <summary>
    /// Gets the total number of work items.
    /// </summary>
    public int Total { get; }

    #endregion
}