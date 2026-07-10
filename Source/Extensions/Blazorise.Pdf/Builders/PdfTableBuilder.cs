#region Using directives
using System;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Builds PDF table definitions.
/// </summary>
public sealed class PdfTableBuilder : PdfElementBuilder
{
    #region Members

    private readonly PdfElementDefinition definition;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new PDF table builder.
    /// </summary>
    /// <param name="definition">The table definition.</param>
    public PdfTableBuilder( PdfElementDefinition definition )
        : base( definition )
    {
        this.definition = definition ?? throw new ArgumentNullException( nameof( definition ) );
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds a row to the table.
    /// </summary>
    /// <param name="height">The row height.</param>
    /// <param name="configure">The row configuration.</param>
    /// <returns>The table builder.</returns>
    public PdfTableBuilder Row( double height, Action<PdfTableRowBuilder> configure )
    {
        if ( configure is null )
            throw new ArgumentNullException( nameof( configure ) );

        PdfTableRowDefinition row = new()
        {
            Height = height,
        };

        definition.Rows.Add( row );
        configure( new( row ) );

        return this;
    }

    #endregion
}