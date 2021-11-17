namespace Blazorise.Animate
{
    /// <summary>
    /// Holds the list of all supported animations.
    /// </summary>
    public static class Animations
    {
        public static IAnimation Fade => new FadeAnimation();
        public static IAnimation FadeIn => new FadeAnimation();
        public static IAnimation FadeUp => new FadeUpAnimation();
        public static IAnimation FadeDown => new FadeDownAnimation();
        public static IAnimation FadeLeft => new FadeLeftAnimation();
        public static IAnimation FadeRight => new FadeRightAnimation();
        public static IAnimation FadeUpRight => new FadeUpRightAnimation();
        public static IAnimation FadeUpLeft => new FadeUpLeftAnimation();
        public static IAnimation FadeDownRight => new FadeDownRightAnimation();
        public static IAnimation FadeDownLeft => new FadeDownLeftAnimation();
        public static IAnimation FlipUp => new FlipUpAnimation();
        public static IAnimation FlipDown => new FlipDownAnimation();
        public static IAnimation FlipLeft => new FlipLeftAnimation();
        public static IAnimation FlipRight => new FlipRightAnimation();
        public static IAnimation SlideUp => new SlideUpAnimation();
        public static IAnimation SlideDown => new SlideDownAnimation();
        public static IAnimation SlideLeft => new SlideLeftAnimation();
        public static IAnimation SlideRight => new SlideRightAnimation();
        public static IAnimation ZoomIn => new ZoomInAnimation();
        public static IAnimation ZoomInUp => new ZoomInUpAnimation();
        public static IAnimation ZoomInDown => new ZoomInDownAnimation();
        public static IAnimation ZoomInLeft => new ZoomInLeftAnimation();
        public static IAnimation ZoomInRight => new ZoomInRightAnimation();
        public static IAnimation ZoomOut => new ZoomOutAnimation();
        public static IAnimation ZoomOutUp => new ZoomOutUpAnimation();
        public static IAnimation ZoomOutDown => new ZoomOutDownAnimation();
        public static IAnimation ZoomOutLeft => new ZoomOutLeftAnimation();
        public static IAnimation ZoomOutRight => new ZoomOutRightAnimation();
    }
}
