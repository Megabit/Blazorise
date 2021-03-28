#region Using directives
#endregion

using System;

namespace Blazorise
{
    public class ThemeSwitchOptions : BasicOptions
    {
        public float BoxShadowLightenColor { get; set; } = 25;

        public float DisabledLightenColor { get; set; } = 50;

        public override bool Equals( object obj )
        {
            return obj is ThemeSwitchOptions options &&
                    base.Equals( obj ) &&
                     BoxShadowLightenColor == options.BoxShadowLightenColor &&
                     DisabledLightenColor == options.DisabledLightenColor;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( base.GetHashCode(), BoxShadowLightenColor, DisabledLightenColor );
        }
    }
}
