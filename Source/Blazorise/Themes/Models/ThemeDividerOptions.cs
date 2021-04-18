#region Using directives
#endregion

using System;

namespace Blazorise
{
    public class ThemeDividerOptions
    {
        public string Color { get; set; } = "#999999";

        public string Thickness { get; set; } = "1px";

        public string TextSize { get; set; } = ".85rem";

        public DividerType? DividerType { get; set; } = Blazorise.DividerType.Solid;

        public override bool Equals( object obj )
        {
            return obj is ThemeDividerOptions options &&
                     Color == options.Color &&
                     Thickness == options.Thickness &&
                     TextSize == options.TextSize &&
                     DividerType == options.DividerType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( Color, Thickness, TextSize, DividerType );
        }
    }
}
