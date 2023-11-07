#region Using directives
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Base class for internal date time formaters.
/// </summary>
public abstract class BaseDateTimeFormatConverter
{
    #region Methods

    /// <inheritdoc/>
    public string Convert( string format )
    {
        if ( string.IsNullOrEmpty( format ) )
            return format;

        var tokens = ParseTokens( format );

        var endFormat = new StringBuilder();

        foreach ( var token in tokens )
        {
            if ( FormatTokens.TryGetValue( token, out var internalToken ) )
                endFormat.Append( internalToken );
            else
                endFormat.Append( token );
        }

        return endFormat.ToString();
    }

    private IList<string> ParseTokens( string format )
    {
        var tokens = new List<string>();

        string tempToken = null;
        char? last = null;

        foreach ( var c in format )
        {
            if ( !char.IsLetter( c ) )
            {
                if ( tempToken != null )
                    tokens.Add( tempToken );

                tokens.Add( c.ToString() );

                tempToken = null;
                last = null;
                continue;
            }

            if ( last == null || ( last != null && last == c ) )
            {
                tempToken ??= string.Empty;

                tempToken += c;
            }

            last = c;
        }

        if ( !string.IsNullOrEmpty( tempToken ) )
        {
            tokens.Add( tempToken );
        }

        return tokens;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the list of mapped format tokens.
    /// </summary>
    protected abstract Dictionary<string, string> FormatTokens { get; }

    #endregion
}
