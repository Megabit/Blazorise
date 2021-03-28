#region Using directives
#endregion

using System;

namespace Blazorise
{
    public class ThemeSidebarOptions
    {
        public string Width { get; set; } = "230px";

        public string BackgroundColor { get; set; } = "#343a40";

        public string Color { get; set; } = "#ced4da";

        public override bool Equals( object obj )
        {
            return obj is ThemeSidebarOptions options &&
                     Width == options.Width &&
                     BackgroundColor == options.BackgroundColor &&
                     Color == options.Color;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( Width, BackgroundColor, Color );
        }
    }
}
