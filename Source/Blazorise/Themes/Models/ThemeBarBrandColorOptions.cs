#region Using directives
#endregion

using System;

namespace Blazorise
{
    public class ThemeBarBrandColorOptions
    {
        public string BackgroundColor { get; set; }

        public override bool Equals( object obj )
        {
            return obj is ThemeBarBrandColorOptions options &&
                     BackgroundColor == options.BackgroundColor;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( BackgroundColor );
        }
    }
}
