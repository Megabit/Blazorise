#region Using directives
#endregion

using System;

namespace Blazorise
{
    public class ThemeBreadcrumbOptions : BasicOptions
    {
        public string Color { get; set; } = ThemeColors.Blue.Shades["400"].Value;

        public override bool Equals( object obj )
        {
            return obj is ThemeBreadcrumbOptions options &&
                    base.Equals( obj ) &&
                     Color == options.Color;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( base.GetHashCode(), Color );
        }
    }
}
