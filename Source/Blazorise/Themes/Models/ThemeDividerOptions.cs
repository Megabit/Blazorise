namespace Blazorise
{
    public record ThemeDividerOptions
    {
        public string Color { get; set; } = "#999999";

        public string Thickness { get; set; } = "1px";

        public string TextSize { get; set; } = ".85rem";

        public DividerType? DividerType { get; set; } = Blazorise.DividerType.Solid;
    }
}
