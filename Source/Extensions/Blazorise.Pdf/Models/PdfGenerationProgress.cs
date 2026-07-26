#region Using directives
using System;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Describes the current PDF generation progress.
/// </summary>
public sealed class PdfGenerationProgress
{
    #region Constructors

    /// <summary>
    /// Initializes a new PDF generation progress state.
    /// </summary>
    /// <param name="stage">The current generation stage.</param>
    /// <param name="progress">The total progress in the range from 0 to 1.</param>
    /// <param name="completedPages">The number of pages that have been rendered.</param>
    /// <param name="totalPages">The total number of pages.</param>
    public PdfGenerationProgress( PdfGenerationStage stage, double progress, int completedPages, int totalPages )
    {
        Stage = stage;
        Progress = Math.Clamp( progress, 0, 1 );
        CompletedPages = Math.Max( 0, completedPages );
        TotalPages = Math.Max( 0, totalPages );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current generation stage.
    /// </summary>
    public PdfGenerationStage Stage { get; }

    /// <summary>
    /// Gets the total progress in the range from 0 to 1.
    /// </summary>
    public double Progress { get; }

    /// <summary>
    /// Gets the total progress in the range from 0 to 100.
    /// </summary>
    public double Percentage => Progress * 100d;

    /// <summary>
    /// Gets the number of pages that have been rendered.
    /// </summary>
    public int CompletedPages { get; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages { get; }

    #endregion
}