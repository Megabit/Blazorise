#region Using directives
#endregion

using System;

namespace Blazorise
{
    public class ThemeDropdownOptions : BasicOptions
    {
        public float GradientBlendPercentage { get; set; } = 15f;

        public bool ToggleIconVisible { get; set; } = true;

        public override bool Equals( object obj )
        {
            return obj is ThemeDropdownOptions options &&
                    base.Equals( obj ) &&
                     GradientBlendPercentage == options.GradientBlendPercentage &&
                     ToggleIconVisible == options.ToggleIconVisible;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( base.GetHashCode(), GradientBlendPercentage, ToggleIconVisible );
        }
    }
}
