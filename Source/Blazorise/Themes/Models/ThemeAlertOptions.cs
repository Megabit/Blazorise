namespace Blazorise
{
    public record ThemeAlertOptions : ThemeBasicOptions
    {
        public int BackgroundLevel { get; set; } = -10;

        public int BorderLevel { get; set; } = -7;

        public int ColorLevel { get; set; } = 6;

        public float GradientBlendPercentage { get; set; } = 15f;
    }
}
