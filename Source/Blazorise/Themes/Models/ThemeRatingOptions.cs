using System;

namespace Blazorise
{
    public class ThemeRatingOptions : BasicOptions
    {
        public float? HoverOpacity { get; set; } = 0.7f;

        public override bool Equals( object obj )
        {
            return obj is ThemeRatingOptions options &&
                    base.Equals( obj ) &&
                     HoverOpacity == options.HoverOpacity;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( base.GetHashCode(), HoverOpacity );
        }
    }
}
