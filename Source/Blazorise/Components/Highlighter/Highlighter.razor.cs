#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Highlights the part of the text based on a search term.
    /// </summary>
    public partial class Highlighter : BaseComponent
    {
        #region Members

        private const string NextBoundary = ".*?\\b";

        IEnumerable<string> fragments;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void OnParametersSet()
        {
            fragments = GetFragments( Text, HighlightedText, CaseSensitive, UntilNextBoundary );
        }

        private static IEnumerable<string> GetFragments( string text, string highlightedText, bool caseSensitive = false, bool untilNextBoundary = false )
        {
            if ( string.IsNullOrWhiteSpace( text ) )
            {
                return new List<string>();
            }

            if ( string.IsNullOrWhiteSpace( highlightedText ) )
            {
                return new List<string> { text };
            }

            highlightedText = Regex.Escape( highlightedText );

            if ( untilNextBoundary )
            {
                highlightedText += NextBoundary;
            }

            return Regex
                .Split( text, $"({highlightedText})", caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase )
                .Where( s => !string.IsNullOrEmpty( s ) );
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
        /// Whether or not the search term will be case sensitive.
        /// </summary>
        [Parameter] public bool CaseSensitive { get; set; }

        /// <summary>
        /// If true, highlights the text until the next word boundary.
        /// </summary>
        [Parameter] public bool UntilNextBoundary { get; set; }

        #endregion
    }
}
