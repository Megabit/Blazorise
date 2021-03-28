#region Using directives
#endregion

using System;

namespace Blazorise
{
    public class ThemeTableOptions : BasicOptions
    {
        public int BackgroundLevel { get; set; } = -9;

        public int BorderLevel { get; set; } = -6;

        public override bool Equals( object obj )
        {
            return obj is ThemeTableOptions options &&
                    base.Equals( obj ) &&
                     BackgroundLevel == options.BackgroundLevel &&
                     BorderLevel == options.BorderLevel;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( base.GetHashCode(), BackgroundLevel, BorderLevel );
        }
    }
}
