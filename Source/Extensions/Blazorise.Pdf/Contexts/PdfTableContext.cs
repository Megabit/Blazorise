#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Provides the current declarative PDF table context.
/// </summary>
public sealed class PdfTableContext
{
    /// <summary>
    /// Initializes a new PDF table context.
    /// </summary>
    /// <param name="definition">The current table definition.</param>
    public PdfTableContext( PdfElementDefinition definition )
    {
        Definition = definition;
    }

    /// <summary>
    /// Current table definition.
    /// </summary>
    public PdfElementDefinition Definition { get; }

    /// <summary>
    /// Rows rendered by the current table.
    /// </summary>
    public IList<PdfTableRowDefinition> Rows => Definition.Rows;
}