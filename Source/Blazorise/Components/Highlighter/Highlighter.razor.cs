#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Highlights the part of the text based on a search term.
/// </summary>
public partial class Highlighter : BaseComponent
{
    #region Objects

    /// <summary>
    /// Defines a fragment of the text.
    /// </summary>
    /// <param name="Text">The text fragment.</param>
    /// <param name="IsMatch">Whether or not the text fragment is a match.</param>
    record Fragment( string Text, bool IsMatch );

    #endregion

    #region Members

    IEnumerable<Fragment> fragments;

    private List<string> allHighlightedTexts = new();

    #endregion

    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var changed =
            ( parameters.TryGetValue<string>( nameof( Text ), out var text ) && Text != text )
            || ( parameters.TryGetValue<string>( nameof( HighlightedText ), out var highlightedText ) && HighlightedText != highlightedText )
            || ( parameters.TryGetValue<bool>( nameof( CaseSensitive ), out var caseSensitive ) && CaseSensitive != caseSensitive )
            || ( parameters.TryGetValue<bool>( nameof( UntilNextBoundary ), out var untilNext ) && UntilNextBoundary != untilNext )
            || ( parameters.TryGetValue<string>( nameof( NextBoundary ), out var nextBoundary ) && NextBoundary != nextBoundary )
            || ( parameters.TryGetValue<IEnumerable<string>>( nameof( HighlightedTexts ), out var highlightedTexts ) && !HighlightedTexts.AreEqual( highlightedTexts ) );

        await base.SetParametersAsync( parameters );

        if ( changed )
        {
            allHighlightedTexts = new( HighlightedTexts ?? Enumerable.Empty<string>() ) { HighlightedText };
            fragments = GetFragments( Text, allHighlightedTexts, CaseSensitive, UntilNextBoundary, NextBoundary );
        }
    }

    static IEnumerable<Fragment> GetFragments( string text, List<string> highlightedTexts, bool caseSensitive = false, bool untilNextBoundary = false, string nextBoundary = null )
    {
        if ( string.IsNullOrWhiteSpace( text ) )
            return new List<Fragment>();

        if ( highlightedTexts == null || highlightedTexts.Count == 0 || highlightedTexts.All( string.IsNullOrWhiteSpace ) )
            return new List<Fragment> { new( text, false ) };

        var escaped = highlightedTexts
                      .Where( s => !string.IsNullOrWhiteSpace( s ) )
                      .Select( Regex.Escape ).ToList();

        string pattern = untilNextBoundary
            ? string.Join( "|", escaped.Select( h => h + nextBoundary ) )
            : string.Join( "|", escaped );

        var regex = new Regex( $"({pattern})", caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase );

        return regex
               .Split( text )
               .Where( s => !string.IsNullOrEmpty( s ) )
               .Select( s => new Fragment( s, regex.IsMatch( s ) ) );
    }

    #endregion

    #region Properties

    /// <summary>
    /// The whole text in which a <see cref="HighlightedText"/> will be highlighted.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// The search term to be highlighted.
    /// </summary>
    [Parameter] public string HighlightedText { get; set; }

    /// <summary>
    /// Array of search terms to be highlighted.
    /// </summary>
    [Parameter] public List<string> HighlightedTexts { get; set; }

    /// <summary>
    /// Whether or not the search term will be case sensitive.
    /// </summary>
    [Parameter] public bool CaseSensitive { get; set; }

    /// <summary>
    /// A regex expression used for searching the word boundaries.
    /// </summary>
    [Parameter] public string NextBoundary { get; set; } = ".*?\\b";

    /// <summary>
    /// If true, highlights the text until the next word boundary.
    /// </summary>
    [Parameter] public bool UntilNextBoundary { get; set; }

    #endregion    
}