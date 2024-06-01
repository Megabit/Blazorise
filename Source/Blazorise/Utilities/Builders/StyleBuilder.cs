#region Using directives
using System;
using System.Text;
using Blazorise.Extensions;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Helper class for easier building of html styles with additional conditions and rules.
/// </summary>
public class StyleBuilder
{
    #region Members

    const char Delimiter = ';';

    private readonly Action<StyleBuilder> buildStyles;

    private StringBuilder builder = new();

    private string styles;

    private bool dirty = true;

    #endregion

    #region Constructors

    /// <summary>
    /// Default style builder constructor that accepts build action.
    /// </summary>
    /// <param name="buildStyles">Action responsible for building the styles.</param>
    public StyleBuilder( Action<StyleBuilder> buildStyles )
    {
        this.buildStyles = buildStyles;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Appends a copy of the specified string to this instance.
    /// </summary>
    /// <param name="value">The string to append.</param>
    public void Append( string value )
    {
        if ( value is not null )
            builder.Append( value ).Append( Delimiter );
    }

    /// <summary>
    /// Appends a copy of the specified string to this instance if <paramref name="condition"/> is true.
    /// </summary>
    /// <param name="value">The string to append.</param>
    /// <param name="condition">Condition that must be true.</param>
    public void Append( string value, bool condition )
    {
        if ( value is not null && condition )
            builder.Append( value ).Append( Delimiter );
    }

    /// <summary>
    /// Marks the builder as dirty to rebuild the values.
    /// </summary>
    public void Dirty()
    {
        dirty = true;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Get the styles.
    /// </summary>
    public string Styles
    {
        get
        {
            if ( dirty )
            {
                builder.Clear();

                buildStyles( this );

                styles = builder.ToString().TrimEnd( ' ', Delimiter )?.EmptyToNull();

                dirty = false;
            }

            return styles;
        }
    }

    #endregion
}