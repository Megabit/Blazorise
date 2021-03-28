#region Using directives
#endregion

using System;

namespace Blazorise
{
    public class ThemePaginationOptions : BasicOptions
    {
        public string LargeBorderRadius { get; set; } = ".3rem";

        public override bool Equals( object obj )
        {
            return obj is ThemePaginationOptions options &&
                    base.Equals( obj ) &&
                     LargeBorderRadius == options.LargeBorderRadius;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( base.GetHashCode(), LargeBorderRadius );
        }
    }
}
