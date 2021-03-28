#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    public class ThemeBreakpointOptions : IEnumerable<KeyValuePair<string, Func<string>>>
    {
        private Dictionary<string, Func<string>> breakpointMap => new Dictionary<string, Func<string>> {
            { "mobile", () => Mobile },
            { "tablet", () => Tablet },
            { "desktop", () => Desktop },
            { "widescreen", () => Widescreen },
            { "fullhd", () => FullHD },
        };

        public IEnumerator<KeyValuePair<string, Func<string>>> GetEnumerator()
        {
            return breakpointMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return breakpointMap.GetEnumerator();
        }

        public Func<string> this[string key] => breakpointMap[key];

        public string Mobile { get; set; } = "576px";

        public string Tablet { get; set; } = "768px";

        public string Desktop { get; set; } = "992px";

        public string Widescreen { get; set; } = "1200px";

        public string FullHD { get; set; } = "1400px";

        public override bool Equals( object obj )
        {
            return obj is ThemeBreakpointOptions options &&
                     Mobile == options.Mobile &&
                     Tablet == options.Tablet &&
                     Desktop == options.Desktop &&
                     Widescreen == options.Widescreen &&
                     FullHD == options.FullHD;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( Mobile, Tablet, Desktop, Widescreen, FullHD );
        }
    }
}
