#region Using directives
#endregion

using System;

namespace Blazorise
{
    public class ThemeButtonOptions : BasicOptions
    {
        public string Padding { get; set; }

        public string Margin { get; set; }

        public string BoxShadowSize { get; set; }

        public byte BoxShadowTransparency { get; set; } = 127;

        public float HoverDarkenColor { get; set; } = 15f;

        public float HoverLightenColor { get; set; } = 20f;

        public float ActiveDarkenColor { get; set; } = 20f;

        public float ActiveLightenColor { get; set; } = 25f;

        public string LargeBorderRadius { get; set; } = ".3rem";

        public string SmallBorderRadius { get; set; } = ".2rem";

        public float GradientBlendPercentage { get; set; } = 15f;

        public override bool Equals( object obj )
        {
            return obj is ThemeButtonOptions options &&
                     base.Equals( obj ) &&
                     Padding == options.Padding &&
                     Margin == options.Margin &&
                     BoxShadowSize == options.BoxShadowSize &&
                     BoxShadowTransparency == options.BoxShadowTransparency &&
                     HoverDarkenColor == options.HoverDarkenColor &&
                     HoverLightenColor == options.HoverLightenColor &&
                     ActiveDarkenColor == options.ActiveDarkenColor &&
                     ActiveLightenColor == options.ActiveLightenColor &&
                     LargeBorderRadius == options.LargeBorderRadius &&
                     SmallBorderRadius == options.SmallBorderRadius &&
                     GradientBlendPercentage == options.GradientBlendPercentage &&
                     DisabledOpacity == options.DisabledOpacity;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add( base.GetHashCode() );
            hash.Add( Padding );
            hash.Add( Margin );
            hash.Add( BoxShadowSize );
            hash.Add( BoxShadowTransparency );
            hash.Add( HoverDarkenColor );
            hash.Add( HoverLightenColor );
            hash.Add( ActiveDarkenColor );
            hash.Add( ActiveLightenColor );
            hash.Add( LargeBorderRadius );
            hash.Add( SmallBorderRadius );
            hash.Add( GradientBlendPercentage );
            hash.Add( DisabledOpacity );
            return hash.ToHashCode();
        }

        public float? DisabledOpacity { get; set; } = .65f;
    }
}
