#region Using directives
#endregion

using System;

namespace Blazorise
{
    public class ThemeAlertOptions : BasicOptions
    {
        public int BackgroundLevel { get; set; } = -10;

        public int BorderLevel { get; set; } = -7;

        public int ColorLevel { get; set; } = 6;

        public float GradientBlendPercentage { get; set; } = 15f;

        public override bool Equals( object obj )
        {
            return obj is ThemeAlertOptions options &&
                    base.Equals( obj ) &&
                     BackgroundLevel == options.BackgroundLevel &&
                     BorderLevel == options.BorderLevel &&
                     ColorLevel == options.ColorLevel &&
                     GradientBlendPercentage == options.GradientBlendPercentage;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( base.GetHashCode(), BackgroundLevel, BorderLevel, ColorLevel, GradientBlendPercentage );
        }
    }
}
