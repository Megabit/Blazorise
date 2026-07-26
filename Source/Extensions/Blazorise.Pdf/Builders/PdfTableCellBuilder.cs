#region Using directives
using System;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Builds PDF table cell definitions.
/// </summary>
public sealed class PdfTableCellBuilder
{
    #region Members

    private readonly PdfTableCellDefinition definition;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new PDF table cell builder.
    /// </summary>
    /// <param name="definition">The cell definition.</param>
    public PdfTableCellBuilder( PdfTableCellDefinition definition )
    {
        this.definition = definition ?? throw new ArgumentNullException( nameof( definition ) );
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds text to the cell.
    /// </summary>
    /// <param name="text">The text value.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder Text( string text )
    {
        PdfElementDefinition element = new()
        {
            Type = PdfElementType.Text,
            Text = text,
            Width = definition.Width,
            Height = 24,
        };

        definition.Elements.Add( element );

        return new( element );
    }

    /// <summary>
    /// Adds an element to the cell.
    /// </summary>
    /// <param name="type">The element type.</param>
    /// <param name="configure">The element configuration.</param>
    /// <returns>The cell builder.</returns>
    public PdfTableCellBuilder Element( PdfElementType type, Action<PdfElementBuilder> configure )
    {
        if ( configure is null )
            throw new ArgumentNullException( nameof( configure ) );

        PdfElementDefinition element = new()
        {
            Type = type,
            Width = definition.Width,
            Height = 24,
        };

        definition.Elements.Add( element );
        configure( new( element ) );

        return this;
    }

    #endregion
}