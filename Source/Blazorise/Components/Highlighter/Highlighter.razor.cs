#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Highlights the part of the text based on a search term.
/// </summary>
public partial class Highlighter : BaseComponent
{
    #region Members

    IEnumerable<string> fragments;
    private List<string> allHighlightedTexts= new();

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        allHighlightedTexts = new List<string>(HighlightedTexts ?? Enumerable.Empty<string>()) { HighlightedText };
        fragments = GetFragments( Text,allHighlightedTexts, CaseSensitive, UntilNextBoundary, NextBoundary );
    }

    static IEnumerable<string> GetFragments( string text, List<string> highlightedTexts, bool caseSensitive = false, bool untilNextBoundary = false, string nextBoundary = null)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new List<string>();

        if (highlightedTexts == null || highlightedTexts.Count == 0 || highlightedTexts.All(string.IsNullOrWhiteSpace))
            return new List<string> { text };

        var escaped = highlightedTexts
                      .Where(s => !string.IsNullOrWhiteSpace(s))
                      .Select(Regex.Escape).ToList();

        string pattern = untilNextBoundary && !string.IsNullOrEmpty(nextBoundary) 
          ? string.Join("|", escaped.Select(h => h + nextBoundary))
          : string.Join("|", escaped);

        var options = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;

        return Regex
               .Split(text, $"({pattern})", options)
               .Where(s => !string.IsNullOrEmpty(s));
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