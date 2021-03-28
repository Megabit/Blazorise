#region Using directives
#endregion

using System;

namespace Blazorise
{
    public class ThemeProgressOptions : BasicOptions
    {
        public string PageProgressDefaultColor { get; set; } = "#ffffff";

        public override bool Equals( object obj )
        {
            return obj is ThemeProgressOptions options &&
                    base.Equals( obj ) &&
                     PageProgressDefaultColor == options.PageProgressDefaultColor;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( base.GetHashCode(), PageProgressDefaultColor );
        }
    }
}
