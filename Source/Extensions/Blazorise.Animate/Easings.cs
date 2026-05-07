namespace Blazorise.Animate;

/// <summary>
/// Holds the list of all supported easings.
/// </summary>
public static class Easings
{
    public static IEasing Linear => Create( "linear" );
    public static IEasing Ease => Create( "ease" );
    public static IEasing EaseIn => Create( "ease-in" );
    public static IEasing EaseOut => Create( "ease-out" );
    public static IEasing EaseInOut => Create( "ease-in-out" );
    public static IEasing EaseInBack => Create( "ease-in-back" );
    public static IEasing EaseOutBack => Create( "ease-out-back" );
    public static IEasing EaseInOutBack => Create( "ease-in-out-back" );
    public static IEasing EaseInSine => Create( "ease-in-sine" );
    public static IEasing EaseOutSine => Create( "ease-out-sine" );
    public static IEasing EaseInOutSine => Create( "ease-in-out-sine" );
    public static IEasing EaseInQuad => Create( "ease-in-quad" );
    public static IEasing EaseOutQuad => Create( "ease-out-quad" );
    public static IEasing EaseInOutQuad => Create( "ease-in-out-quad" );
    public static IEasing EaseInCubic => Create( "ease-in-cubic" );
    public static IEasing EaseOutCubic => Create( "ease-out-cubic" );
    public static IEasing EaseInOutCubic => Create( "ease-in-out-cubic" );
    public static IEasing EaseInQuart => Create( "ease-in-quart" );
    public static IEasing EaseOutQuart => Create( "ease-out-quart" );
    public static IEasing EaseInOutQuart => Create( "ease-in-out-quart" );

    private static IEasing Create( string name )
        => new EasingDefinition( name, GetValue( name ) );

    internal static object GetValue( IEasing easing )
    {
        if ( easing is IEasingDefinition easingDefinition )
        {
            return easingDefinition.Value;
        }

        return GetValue( easing?.Name );
    }

    internal static object GetValue( string name )
    {
        return name switch
        {
            "linear" => "linear",
            "ease" => CubicBezier( 0.25, 0.1, 0.25, 1 ),
            "ease-in" => "easeIn",
            "ease-out" => "easeOut",
            "ease-in-out" => "easeInOut",
            "ease-in-back" => CubicBezier( 0.6, -0.28, 0.735, 0.045 ),
            "ease-out-back" => CubicBezier( 0.175, 0.885, 0.32, 1.275 ),
            "ease-in-out-back" => CubicBezier( 0.68, -0.55, 0.265, 1.55 ),
            "ease-in-sine" => CubicBezier( 0.47, 0, 0.745, 0.715 ),
            "ease-out-sine" => CubicBezier( 0.39, 0.575, 0.565, 1 ),
            "ease-in-out-sine" => CubicBezier( 0.445, 0.05, 0.55, 0.95 ),
            "ease-in-quad" => CubicBezier( 0.55, 0.085, 0.68, 0.53 ),
            "ease-out-quad" => CubicBezier( 0.25, 0.46, 0.45, 0.94 ),
            "ease-in-out-quad" => CubicBezier( 0.455, 0.03, 0.515, 0.955 ),
            "ease-in-cubic" => CubicBezier( 0.55, 0.085, 0.68, 0.53 ),
            "ease-out-cubic" => CubicBezier( 0.25, 0.46, 0.45, 0.94 ),
            "ease-in-out-cubic" => CubicBezier( 0.455, 0.03, 0.515, 0.955 ),
            "ease-in-quart" => CubicBezier( 0.55, 0.085, 0.68, 0.53 ),
            "ease-out-quart" => CubicBezier( 0.25, 0.46, 0.45, 0.94 ),
            "ease-in-out-quart" => CubicBezier( 0.455, 0.03, 0.515, 0.955 ),
            _ => null,
        };
    }

    private static double[] CubicBezier( double x1, double y1, double x2, double y2 )
        => new[] { x1, y1, x2, y2 };
}