namespace Blazorise.Animate
{
    /// <summary>
    /// Holds the list of all supported easings.
    /// </summary>
    public static class Easings
    {
        public static IEasing Linear => new LinearEasing();
        public static IEasing Ease => new EaseEasing();
        public static IEasing EaseIn => new EaseInEasing();
        public static IEasing EaseOut => new EaseOutEasing();
        public static IEasing EaseInOut => new EaseInOutEasing();
        public static IEasing EaseInBack => new EaseInBackEasing();
        public static IEasing EaseOutBack => new EaseOutBackEasing();
        public static IEasing EaseInOutBack => new EaseInOutBackEasing();
        public static IEasing EaseInSine => new EaseInSineEasing();
        public static IEasing EaseOutSine => new EaseOutSineEasing();
        public static IEasing EaseInOutSine => new EaseInOutSineEasing();
        public static IEasing EaseInQuad => new EaseInQuadEasing();
        public static IEasing EaseOutQuad => new EaseOutQuadEasing();
        public static IEasing EaseInOutQuad => new EaseInOutQuadEasing();
        public static IEasing EaseInCubic => new EaseInCubicEasing();
        public static IEasing EaseOutCubic => new EaseOutCubicEasing();
        public static IEasing EaseInOutCubic => new EaseInOutCubicEasing();
        public static IEasing EaseInQuart => new EaseInQuartEasing();
        public static IEasing EaseOutQuart => new EaseOutQuartEasing();
        public static IEasing EaseInOutQuart => new EaseInOutQuartEasing();
    }
}
