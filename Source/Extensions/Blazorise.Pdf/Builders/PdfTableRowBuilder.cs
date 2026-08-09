#region Using directives
using System;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Builds PDF table row definitions.
/// </summary>
public sealed class PdfTableRowBuilder
{
    #region Members

    private readonly PdfTableRowDefinition definition;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new PDF table row builder.
    /// </summary>
    /// <param name="definition">The row definition.</param>
    public PdfTableRowBuilder( PdfTableRowDefinition definition )
    {
        this.definition = definition ?? throw new ArgumentNullException( nameof( definition ) );
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds a cell to the row.
    /// </summary>
    /// <param name="width">The cell width.</param>
    /// <param name="configure">The cell configuration.</param>
    /// <returns>The row builder.</returns>
    public PdfTableRowBuilder Cell( double width, Action<PdfTableCellBuilder> configure = null )
    {
        PdfTableCellDefinition cell = new()
        {
            Width = width,
        };

        definition.Cells.Add( cell );
        configure?.Invoke( new( cell ) );

        return this;
    }

    #endregion
}