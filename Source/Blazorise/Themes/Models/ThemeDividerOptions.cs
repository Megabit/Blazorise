#region Using directives
#endregion

namespace Blazorise
{
    public class ThemeDividerOptions
    {
        public string Color { get; set; } = "#999999";

        public string Thickness { get; set; } = "2px";

        public string TextSize { get; set; } = ".85rem";

        public DividerType? DividerType { get; set; } = Blazorise.DividerType.Solid;
    }
}
