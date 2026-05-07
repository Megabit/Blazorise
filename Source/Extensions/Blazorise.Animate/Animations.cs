#region Using directives
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Animate;

/// <summary>
/// Holds the list of all supported animations.
/// </summary>
public static class Animations
{
    public static IAnimation Fade => Create( "fade" );
    public static IAnimation FadeIn => Create( "fade" );
    public static IAnimation FadeUp => Create( "fade-up" );
    public static IAnimation FadeDown => Create( "fade-down" );
    public static IAnimation FadeLeft => Create( "fade-left" );
    public static IAnimation FadeRight => Create( "fade-right" );
    public static IAnimation FadeUpRight => Create( "fade-up-right" );
    public static IAnimation FadeUpLeft => Create( "fade-up-left" );
    public static IAnimation FadeDownRight => Create( "fade-down-right" );
    public static IAnimation FadeDownLeft => Create( "fade-down-left" );
    public static IAnimation FlipUp => Create( "flip-up" );
    public static IAnimation FlipDown => Create( "flip-down" );
    public static IAnimation FlipLeft => Create( "flip-left" );
    public static IAnimation FlipRight => Create( "flip-right" );
    public static IAnimation SlideUp => Create( "slide-up" );
    public static IAnimation SlideDown => Create( "slide-down" );
    public static IAnimation SlideLeft => Create( "slide-left" );
    public static IAnimation SlideRight => Create( "slide-right" );
    public static IAnimation ZoomIn => Create( "zoom-in" );
    public static IAnimation ZoomInUp => Create( "zoom-in-up" );
    public static IAnimation ZoomInDown => Create( "zoom-in-down" );
    public static IAnimation ZoomInLeft => Create( "zoom-in-left" );
    public static IAnimation ZoomInRight => Create( "zoom-in-right" );
    public static IAnimation ZoomOut => Create( "zoom-out" );
    public static IAnimation ZoomOutUp => Create( "zoom-out-up" );
    public static IAnimation ZoomOutDown => Create( "zoom-out-down" );
    public static IAnimation ZoomOutLeft => Create( "zoom-out-left" );
    public static IAnimation ZoomOutRight => Create( "zoom-out-right" );

    private static IEnumerable<string> availableAnimationNames = null;

    public static bool TryParse( string value, out IAnimation result )
    {
        try
        {
            var animationProp = typeof( Animations ).GetProperty( value, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static );

            result = (IAnimation)animationProp.GetValue( null, null );

            return true;
        }
        catch
        {
            result = default;

            return false;
        }
    }

    public static IEnumerable<string> GetNames()
    {
        if ( availableAnimationNames == null )
            availableAnimationNames = typeof( Animations ).GetProperties( System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static ).Select( x => x.Name );

        return availableAnimationNames;
    }

    private static IAnimation Create( string name )
        => new AnimationDefinition( name, GetKeyframes( name ) );

    internal static IReadOnlyList<AnimationFrame> GetKeyframes( IAnimation animation )
    {
        if ( animation is IAnimationDefinition animationDefinition )
        {
            return animationDefinition.Keyframes;
        }

        return GetKeyframes( animation?.Name );
    }

    internal static IReadOnlyList<AnimationFrame> GetKeyframes( string name )
    {
        return name switch
        {
            "fade-up" => Keyframes(
                new AnimationFrame { Opacity = 0, Y = "100px" },
                new AnimationFrame { Opacity = 1, Y = "0px" } ),
            "fade-down" => Keyframes(
                new AnimationFrame { Opacity = 0, Y = "-100px" },
                new AnimationFrame { Opacity = 1, Y = "0px" } ),
            "fade-left" => Keyframes(
                new AnimationFrame { Opacity = 0, X = "100px" },
                new AnimationFrame { Opacity = 1, X = "0px" } ),
            "fade-right" => Keyframes(
                new AnimationFrame { Opacity = 0, X = "-100px" },
                new AnimationFrame { Opacity = 1, X = "0px" } ),
            "fade-up-right" => Keyframes(
                new AnimationFrame { Opacity = 0, X = "-100px", Y = "100px" },
                new AnimationFrame { Opacity = 1, X = "0px", Y = "0px" } ),
            "fade-up-left" => Keyframes(
                new AnimationFrame { Opacity = 0, X = "100px", Y = "100px" },
                new AnimationFrame { Opacity = 1, X = "0px", Y = "0px" } ),
            "fade-down-right" => Keyframes(
                new AnimationFrame { Opacity = 0, X = "-100px", Y = "-100px" },
                new AnimationFrame { Opacity = 1, X = "0px", Y = "0px" } ),
            "fade-down-left" => Keyframes(
                new AnimationFrame { Opacity = 0, X = "100px", Y = "-100px" },
                new AnimationFrame { Opacity = 1, X = "0px", Y = "0px" } ),
            "flip-up" => Keyframes(
                new AnimationFrame { Opacity = 0, TransformPerspective = "2500px", RotateX = "-100deg" },
                new AnimationFrame { Opacity = 1, TransformPerspective = "2500px", RotateX = "0deg" } ),
            "flip-down" => Keyframes(
                new AnimationFrame { Opacity = 0, TransformPerspective = "2500px", RotateX = "100deg" },
                new AnimationFrame { Opacity = 1, TransformPerspective = "2500px", RotateX = "0deg" } ),
            "flip-left" => Keyframes(
                new AnimationFrame { Opacity = 0, TransformPerspective = "2500px", RotateY = "-100deg" },
                new AnimationFrame { Opacity = 1, TransformPerspective = "2500px", RotateY = "0deg" } ),
            "flip-right" => Keyframes(
                new AnimationFrame { Opacity = 0, TransformPerspective = "2500px", RotateY = "100deg" },
                new AnimationFrame { Opacity = 1, TransformPerspective = "2500px", RotateY = "0deg" } ),
            "slide-up" => Keyframes(
                new AnimationFrame { Y = "100%" },
                new AnimationFrame { Y = "0%" } ),
            "slide-down" => Keyframes(
                new AnimationFrame { Y = "-100%" },
                new AnimationFrame { Y = "0%" } ),
            "slide-left" => Keyframes(
                new AnimationFrame { X = "100%" },
                new AnimationFrame { X = "0%" } ),
            "slide-right" => Keyframes(
                new AnimationFrame { X = "-100%" },
                new AnimationFrame { X = "0%" } ),
            "zoom-in" => Keyframes(
                new AnimationFrame { Opacity = 0, Scale = 0.6 },
                new AnimationFrame { Opacity = 1, Scale = 1 } ),
            "zoom-in-up" => Keyframes(
                new AnimationFrame { Opacity = 0, Y = "100px", Scale = 0.6 },
                new AnimationFrame { Opacity = 1, Y = "0px", Scale = 1 } ),
            "zoom-in-down" => Keyframes(
                new AnimationFrame { Opacity = 0, Y = "-100px", Scale = 0.6 },
                new AnimationFrame { Opacity = 1, Y = "0px", Scale = 1 } ),
            "zoom-in-left" => Keyframes(
                new AnimationFrame { Opacity = 0, X = "100px", Scale = 0.6 },
                new AnimationFrame { Opacity = 1, X = "0px", Scale = 1 } ),
            "zoom-in-right" => Keyframes(
                new AnimationFrame { Opacity = 0, X = "-100px", Scale = 0.6 },
                new AnimationFrame { Opacity = 1, X = "0px", Scale = 1 } ),
            "zoom-out" => Keyframes(
                new AnimationFrame { Opacity = 0, Scale = 1.2 },
                new AnimationFrame { Opacity = 1, Scale = 1 } ),
            "zoom-out-up" => Keyframes(
                new AnimationFrame { Opacity = 0, Y = "100px", Scale = 1.2 },
                new AnimationFrame { Opacity = 1, Y = "0px", Scale = 1 } ),
            "zoom-out-down" => Keyframes(
                new AnimationFrame { Opacity = 0, Y = "-100px", Scale = 1.2 },
                new AnimationFrame { Opacity = 1, Y = "0px", Scale = 1 } ),
            "zoom-out-left" => Keyframes(
                new AnimationFrame { Opacity = 0, X = "100px", Scale = 1.2 },
                new AnimationFrame { Opacity = 1, X = "0px", Scale = 1 } ),
            "zoom-out-right" => Keyframes(
                new AnimationFrame { Opacity = 0, X = "-100px", Scale = 1.2 },
                new AnimationFrame { Opacity = 1, X = "0px", Scale = 1 } ),
            "fade" or "fade-in" => Keyframes(
                new AnimationFrame { Opacity = 0 },
                new AnimationFrame { Opacity = 1 } ),
            _ => null,
        };
    }

    private static IReadOnlyList<AnimationFrame> Keyframes( params AnimationFrame[] keyframes )
        => keyframes;
}