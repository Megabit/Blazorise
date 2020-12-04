#region Using directives
#endregion

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
    }
}
