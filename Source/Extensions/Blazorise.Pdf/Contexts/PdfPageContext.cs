#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Provides the current declarative PDF page context.
/// </summary>
public sealed class PdfPageContext
{
    /// <summary>
    /// Initializes a new PDF page context.
    /// </summary>
    /// <param name="definition">The current page definition.</param>
    public PdfPageContext( PdfPageDefinition definition )
    {
        Definition = definition;
    }

    /// <summary>
    /// Current page definition.
    /// </summary>
    public PdfPageDefinition Definition { get; }

    /// <summary>
    /// Elements rendered on the current page.
    /// </summary>
    public IList<PdfElementDefinition> Elements => Definition.Elements;
}