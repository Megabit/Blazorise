#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Provides the current declarative PDF document context.
/// </summary>
public sealed class PdfDocumentContext
{
    /// <summary>
    /// Initializes a new PDF document context.
    /// </summary>
    /// <param name="definition">The current document definition.</param>
    public PdfDocumentContext( PdfDocumentDefinition definition )
    {
        Definition = definition;
    }

    /// <summary>
    /// Current document definition.
    /// </summary>
    public PdfDocumentDefinition Definition { get; }
}

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