#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    public class ThemeContainerMaxWidthOptions : IEnumerable<KeyValuePair<string, Func<string>>>
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

        public string Mobile { get; set; } = "540px";

        public string Tablet { get; set; } = "720px";

        public string Desktop { get; set; } = "960px";

        public string Widescreen { get; set; } = "1140px";

        public string FullHD { get; set; } = "1320px";
    }
}
