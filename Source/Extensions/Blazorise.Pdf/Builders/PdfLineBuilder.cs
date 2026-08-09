#region Using directives
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Builds PDF line definitions.
/// </summary>
public sealed class PdfLineBuilder : PdfElementBuilder
{
    #region Constructors

    /// <summary>
    /// Initializes a new PDF line builder.
    /// </summary>
    /// <param name="definition">The line definition.</param>
    public PdfLineBuilder( PdfElementDefinition definition )
        : base( definition )
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Sets the line orientation.
    /// </summary>
    /// <param name="orientation">The line orientation.</param>
    /// <returns>The line builder.</returns>
    public PdfLineBuilder Orientation( Orientation orientation )
    {
        Definition.Orientation = orientation;

        return this;
    }

    #endregion
}