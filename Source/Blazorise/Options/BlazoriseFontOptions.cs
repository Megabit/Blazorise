#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <summary>
/// Defines Blazorise font registration options.
/// </summary>
public sealed class BlazoriseFontOptions
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of Blazorise font options.
    /// </summary>
    public BlazoriseFontOptions()
    {
        foreach ( FontFamily font in Fonts.BuiltIn )
        {
            Add( font );
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds a font family.
    /// </summary>
    /// <param name="font">Font family.</param>
    public void Add( FontFamily font )
    {
        if ( font is not null )
            Families.Add( font );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Default font family name.
    /// </summary>
    public string DefaultFamily { get; set; } = Fonts.Helvetica;

    /// <summary>
    /// Fallback font family name.
    /// </summary>
    public string FallbackFamily { get; set; } = Fonts.Helvetica;

    /// <summary>
    /// Registered font families.
    /// </summary>
    public IList<FontFamily> Families { get; } = [];

    #endregion
}