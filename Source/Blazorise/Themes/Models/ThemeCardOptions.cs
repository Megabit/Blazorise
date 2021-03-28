#region Using directives
#endregion

using System;

namespace Blazorise
{
    public class ThemeCardOptions : BasicOptions
    {
        public string ImageTopRadius { get; set; } = "calc(.25rem - 1px)";

        public override bool Equals( object obj )
        {
            return obj is ThemeCardOptions options &&
                    base.Equals( obj ) &&
                     ImageTopRadius == options.ImageTopRadius;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( base.GetHashCode(), ImageTopRadius );
        }
    }
}
