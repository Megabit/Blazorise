#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    public class ThemeTextColorOptions : IEnumerable<KeyValuePair<string, Func<string>>>
    {
        private Dictionary<string, Func<string>> colorMap => new Dictionary<string, Func<string>> {
            { "primary", () => Primary },
            { "secondary", () => Secondary },
            { "success", () => Success },
            { "info", () => Info },
            { "warning", () => Warning },
            { "danger", () => Danger },
            { "light", () => Light },
            { "dark", () => Dark },
            { "body", () => Body },
            { "muted", () => Muted },
            { "white", () => White },
            { "black50", () => Black50 },
            { "white50", () => White50 },
        };

        public IEnumerator<KeyValuePair<string, Func<string>>> GetEnumerator()
        {
            return colorMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return colorMap.GetEnumerator();
        }

        /// <summary>
        /// Gets the color handler associated with the specified color key.
        /// </summary>
        /// <param name="key">Color key</param>
        /// <returns>Return the color getter.</returns>
        public Func<string> this[string key] => colorMap[key];

        public string Primary { get; set; } = ThemeColors.Blue.Shades["400"].Value;

        public string Secondary { get; set; } = ThemeColors.Gray.Shades["600"].Value;

        public string Success { get; set; } = ThemeColors.Green.Shades["500"].Value;

        public string Info { get; set; } = ThemeColors.Cyan.Shades["500"].Value;

        public string Warning { get; set; } = ThemeColors.Yellow.Shades["500"].Value;

        public string Danger { get; set; } = ThemeColors.Red.Shades["500"].Value;

        public string Light { get; set; } = ThemeColors.Gray.Shades["100"].Value;

        public string Dark { get; set; } = ThemeColors.Gray.Shades["800"].Value;

        public string Body { get; set; } = ThemeColors.Gray.Shades["900"].Value;

        public string Muted { get; set; } = ThemeColors.Gray.Shades["600"].Value;

        public string White { get; set; } = ThemeColors.Gray.Shades["50"].Value;

        public string Black50 { get; set; } = "#000000";

        public string White50 { get; set; } = "#FFFFFF";

        public override bool Equals( object obj )
        {
            return obj is ThemeTextColorOptions options &&
                     Primary == options.Primary &&
                     Secondary == options.Secondary &&
                     Success == options.Success &&
                     Info == options.Info &&
                     Warning == options.Warning &&
                     Danger == options.Danger &&
                     Light == options.Light &&
                     Dark == options.Dark &&
                     Body == options.Body &&
                     Muted == options.Muted &&
                     White == options.White &&
                     Black50 == options.Black50 &&
                     White50 == options.White50;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add( Primary );
            hash.Add( Secondary );
            hash.Add( Success );
            hash.Add( Info );
            hash.Add( Warning );
            hash.Add( Danger );
            hash.Add( Light );
            hash.Add( Dark );
            hash.Add( Body );
            hash.Add( Muted );
            hash.Add( White );
            hash.Add( Black50 );
            hash.Add( White50 );
            return hash.ToHashCode();
        }
    }
}
