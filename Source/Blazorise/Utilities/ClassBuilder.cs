#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using Blazorise.Extensions;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Helper class for easier building of CSS classnames with additional conditions and rules.
/// </summary>
public class ClassBuilder
{
    #region Members

    private const char Delimiter = ' ';

    private readonly Action<ClassBuilder> buildClasses;

    private StringBuilder builder = new();

    private string classNames;

    private bool dirty = true;

    #endregion

    #region Constructors

    /// <summary>
    /// Default class builder constructor that accepts build action.
    /// </summary>
    /// <param name="buildClasses">Action responsible for building the classes.</param>
    public ClassBuilder( Action<ClassBuilder> buildClasses )
    {
        this.buildClasses = buildClasses;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Appends a copy of the specified string to this instance.
    /// </summary>
    /// <param name="value">The string to append.</param>
    public void Append( string value )
    {
        if ( value is null )
            return;

        builder.Append( value ).Append( Delimiter );
    }

    /// <summary>
    /// Appends a copy of the specified string to this instance if <paramref name="condition"/> is true.
    /// </summary>
    /// <param name="value">The string to append.</param>
    /// <param name="condition">Condition that must be true.</param>
    public void Append( string value, bool condition )
    {
        if ( condition && value != null )
            builder.Append( value ).Append( Delimiter );
    }

    /// <summary>
    /// Appends a copy of the specified list of strings to this instance.
    /// </summary>
    /// <param name="values">The list of strings to append.</param>
    public void Append( IEnumerable<string> values )
    {
        builder.Append( string.Join( Delimiter, values ) ).Append( Delimiter );
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
    /// Gets the class-names.
    /// </summary>
    public string Class
    {
        get
        {
            if ( dirty )
            {
                builder.Clear();

                buildClasses( this );

                classNames = builder.ToString().TrimEnd()?.EmptyToNull();

                dirty = false;
            }

            return classNames;
        }
    }

    #endregion
}