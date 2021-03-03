#region Using directives
#endregion

namespace Blazorise
{
    public class ThemeBarOptions
    {
        public string VerticalWidth { get; set; } = "230px";

        public string VerticalSmallWidth { get; set; } = "64px";

        public string VerticalBrandHeight { get; set; } = "64px";

        public string VerticalPopoutMenuWidth { get; set; } = "180px";

        public string HorizontalHeight { get; set; } = "auto";

        public ThemeBarColorOptions DarkColors { get; set; }

        public ThemeBarColorOptions LightColors { get; set; }
    }
}
