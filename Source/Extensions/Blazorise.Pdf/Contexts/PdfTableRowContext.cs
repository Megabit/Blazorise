#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Provides the current declarative PDF table row context.
/// </summary>
public sealed class PdfTableRowContext
{
    /// <summary>
    /// Initializes a new PDF table row context.
    /// </summary>
    /// <param name="definition">The current row definition.</param>
    public PdfTableRowContext( PdfTableRowDefinition definition )
    {
        Definition = definition;
    }

    /// <summary>
    /// Current row definition.
    /// </summary>
    public PdfTableRowDefinition Definition { get; }

    /// <summary>
    /// Cells rendered by the current row.
    /// </summary>
    public IList<PdfTableCellDefinition> Cells => Definition.Cells;
}