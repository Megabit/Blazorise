#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines the map of media breakpoints.
    /// </summary>
    public record ThemeBreakpointOptions : IEnumerable<KeyValuePair<string, Func<string>>>
    {
        /// <summary>
        /// Map of media breakpoints.
        /// </summary>
        private Dictionary<string, Func<string>> BreakpointMap => new()
        {
            { "mobile", () => Mobile },
            { "tablet", () => Tablet },
            { "desktop", () => Desktop },
            { "widescreen", () => Widescreen },
            { "fullhd", () => FullHD },
        };

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, Func<string>>> GetEnumerator()
        {
            return BreakpointMap.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return BreakpointMap.GetEnumerator();
        }

        /// <summary>
        /// Gets the breakpoint handler associated with the specified breakpoint key.
        /// </summary>
        /// <param name="key">Breakpoint key</param>
        /// <returns>Return the breakpoint getter.</returns>
        public Func<string> this[string key] => BreakpointMap[key];

        /// <summary>
        /// Gets or sets the breakpoint size for mobile screens.
        /// </summary>
        public string Mobile { get; set; } = "576px";

        /// <summary>
        /// Gets or sets the breakpoint size for tablet screens.
        /// </summary>
        public string Tablet { get; set; } = "768px";

        /// <summary>
        /// Gets or sets the breakpoint size for desktop screens.
        /// </summary>
        public string Desktop { get; set; } = "992px";

        /// <summary>
        /// Gets or sets the breakpoint size for wide screens.
        /// </summary>
        public string Widescreen { get; set; } = "1200px";

        /// <summary>
        /// Gets or sets the breakpoint size for largest screens.
        /// </summary>
        public string FullHD { get; set; } = "1400px";
    }
}
