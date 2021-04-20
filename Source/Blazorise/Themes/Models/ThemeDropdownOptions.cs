namespace Blazorise
{
    public record ThemeDropdownOptions : ThemeBasicOptions
    {
        public float GradientBlendPercentage { get; set; } = 15f;

        public bool ToggleIconVisible { get; set; } = true;
    }
}
