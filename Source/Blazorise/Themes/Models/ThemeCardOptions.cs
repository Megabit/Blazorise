namespace Blazorise
{
    public record ThemeCardOptions : ThemeBasicOptions
    {
        public string ImageTopRadius { get; set; } = "calc(.25rem - 1px)";
    }
}
