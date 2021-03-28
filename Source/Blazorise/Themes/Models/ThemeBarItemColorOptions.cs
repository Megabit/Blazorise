#region Using directives
#endregion

using System;

namespace Blazorise
{
    public class ThemeBarItemColorOptions
    {
        public string ActiveBackgroundColor { get; set; }

        public string ActiveColor { get; set; }

        public string HoverBackgroundColor { get; set; }

        public string HoverColor { get; set; }

        public override bool Equals( object obj )
        {
            return obj is ThemeBarItemColorOptions options &&
                     ActiveBackgroundColor == options.ActiveBackgroundColor &&
                     ActiveColor == options.ActiveColor &&
                     HoverBackgroundColor == options.HoverBackgroundColor &&
                     HoverColor == options.HoverColor;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( ActiveBackgroundColor, ActiveColor, HoverBackgroundColor, HoverColor );
        }
    }
}
