#region Using directives
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using RegexMatch = System.Text.RegularExpressions.Match;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Compares two strings and find the differences.
/// </summary>
public interface IValidationMessageLocalizerAttributeFinder
{
    /// <summary>
    /// Find all the differences from two string.
    /// </summary>
    /// <param name="first">First string.</param>
    /// <param name="second">Second string.</param>
    /// <returns>Return the list of differences if any is found</returns>
    IEnumerable<(string Index, string Argument)> FindAll( string first, string second );
}

/// <summary>
/// Default implementation of <see cref="IValidationMessageLocalizerAttributeFinder"/>.
/// </summary>
public class ValidationMessageLocalizerAttributeFinder : IValidationMessageLocalizerAttributeFinder
{
    private static readonly Regex PlaceholderRegex = new( @"\{(?<index>\d+)(?:[^}]*)\}", RegexOptions.Compiled );

    #region Methods

    /// <inheritdoc/>
    public virtual IEnumerable<(string Index, string Argument)> FindAll( string first, string second )
    {
        if ( string.IsNullOrEmpty( first ) || string.IsNullOrEmpty( second ) )
            yield break;

        var matches = PlaceholderRegex.Matches( second );

        if ( matches.Count == 0 )
            yield break;

        var indexes = new List<string>( matches.Count );
        var patternBuilder = new StringBuilder();
        var lastIndex = 0;

        foreach ( RegexMatch match in matches )
        {
            patternBuilder.Append( Regex.Escape( second.Substring( lastIndex, match.Index - lastIndex ) ) );
            patternBuilder.Append( "(.*?)" );
            indexes.Add( match.Groups["index"].Value );
            lastIndex = match.Index + match.Length;
        }

        patternBuilder.Append( Regex.Escape( second.Substring( lastIndex ) ) );

        var pattern = "^" + patternBuilder + "$";
        var messageMatch = Regex.Match( first, pattern, RegexOptions.Singleline );

        if ( !messageMatch.Success )
            yield break;

        for ( int i = 0; i < indexes.Count; ++i )
            yield return (indexes[i], messageMatch.Groups[i + 1].Value);
    }

    #endregion
}