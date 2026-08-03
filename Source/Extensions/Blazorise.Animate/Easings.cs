namespace Blazorise.Animate;

/// <summary>
/// Holds the list of all supported easings.
/// </summary>
public static class Easings
{
    /// <summary>
    /// Constant-speed motion from start to finish.
    /// </summary>
    public static IEasing Linear => Create( "linear" );

    /// <summary>
    /// Balanced CSS easing with a gentle start and finish.
    /// </summary>
    public static IEasing Ease => Create( "ease" );

    /// <summary>
    /// Motion that accelerates away from rest.
    /// </summary>
    public static IEasing EaseIn => Create( "ease-in" );

    /// <summary>
    /// Motion that decelerates into its destination.
    /// </summary>
    public static IEasing EaseOut => Create( "ease-out" );

    /// <summary>
    /// Symmetric acceleration followed by deceleration.
    /// </summary>
    public static IEasing EaseInOut => Create( "ease-in-out" );

    /// <summary>
    /// Accelerating motion that first pulls slightly backward.
    /// </summary>
    public static IEasing EaseInBack => Create( "ease-in-back" );

    /// <summary>
    /// Decelerating motion that overshoots its destination.
    /// </summary>
    public static IEasing EaseOutBack => Create( "ease-out-back" );

    /// <summary>
    /// Motion with anticipation at the start and overshoot at the end.
    /// </summary>
    public static IEasing EaseInOutBack => Create( "ease-in-out-back" );

    /// <summary>
    /// Gentle sinusoidal acceleration from rest.
    /// </summary>
    public static IEasing EaseInSine => Create( "ease-in-sine" );

    /// <summary>
    /// Gentle sinusoidal deceleration toward rest.
    /// </summary>
    public static IEasing EaseOutSine => Create( "ease-out-sine" );

    /// <summary>
    /// Smooth sinusoidal motion at both endpoints.
    /// </summary>
    public static IEasing EaseInOutSine => Create( "ease-in-out-sine" );

    /// <summary>
    /// Moderate quadratic acceleration.
    /// </summary>
    public static IEasing EaseInQuad => Create( "ease-in-quad" );

    /// <summary>
    /// Moderate quadratic deceleration.
    /// </summary>
    public static IEasing EaseOutQuad => Create( "ease-out-quad" );

    /// <summary>
    /// Quadratic timing mirrored around the midpoint.
    /// </summary>
    public static IEasing EaseInOutQuad => Create( "ease-in-out-quad" );

    /// <summary>
    /// Strong cubic acceleration into motion.
    /// </summary>
    public static IEasing EaseInCubic => Create( "ease-in-cubic" );

    /// <summary>
    /// Strong cubic deceleration into the endpoint.
    /// </summary>
    public static IEasing EaseOutCubic => Create( "ease-out-cubic" );

    /// <summary>
    /// Cubic acceleration and deceleration around the midpoint.
    /// </summary>
    public static IEasing EaseInOutCubic => Create( "ease-in-out-cubic" );

    /// <summary>
    /// Pronounced quartic acceleration from a slow start.
    /// </summary>
    public static IEasing EaseInQuart => Create( "ease-in-quart" );

    /// <summary>
    /// Pronounced quartic deceleration near completion.
    /// </summary>
    public static IEasing EaseOutQuart => Create( "ease-out-quart" );

    /// <summary>
    /// Quartic timing with a sharp midpoint transition.
    /// </summary>
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