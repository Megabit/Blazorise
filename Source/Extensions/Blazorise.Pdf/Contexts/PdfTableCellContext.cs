#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Provides the current declarative PDF table cell context.
/// </summary>
public sealed class PdfTableCellContext
{
    /// <summary>
    /// Initializes a new PDF table cell context.
    /// </summary>
    /// <param name="definition">The current cell definition.</param>
    public PdfTableCellContext( PdfTableCellDefinition definition )
    {
        Definition = definition;
    }

    /// <summary>
    /// Current cell definition.
    /// </summary>
    public PdfTableCellDefinition Definition { get; }

    /// <summary>
    /// Elements rendered by the current cell.
    /// </summary>
    public IList<PdfElementDefinition> Elements => Definition.Elements;
}