#region Using directives
#endregion

using System;

namespace Blazorise
{
    public class ThemeInputOptions : BasicOptions
    {
        public string Color { get; set; }

        public string CheckColor { get; set; }

        public string SliderColor { get; set; }

        public override bool HasOptions()
        {
            return !string.IsNullOrEmpty( Color )
                || !string.IsNullOrEmpty( CheckColor )
                || !string.IsNullOrEmpty( SliderColor )
                || base.HasOptions();
        }

        public override bool Equals( object obj )
        {
            return obj is ThemeInputOptions options &&
                    base.Equals( obj ) &&
                     Color == options.Color &&
                     CheckColor == options.CheckColor &&
                     SliderColor == options.SliderColor;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( base.GetHashCode(), Color, CheckColor, SliderColor );
        }
    }
}
