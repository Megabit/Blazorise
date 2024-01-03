#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
#endregion

namespace Blazorise.Animate;

/// <summary>
/// Runs animations for any content places inside of <see cref="Animate"/>. component.
/// </summary>
public partial class Animate
{
    #region Members

    /// <summary>
    /// Flag that indicates if animation was manually executed.
    /// </summary>
    private bool manuallyExecuted;

    #endregion

    #region Methods

    /// <summary>
    /// Gets the options from global settings.
    /// </summary>
    /// <returns></returns>
    private AnimateOptions GetOptions()
    {
        var result = Options;

        if ( result != null )
        {
            return result;
        }

        result = OptionsAccessor.Get( OptionsName );

        return result;
    }

    /// <summary>
    /// Runs the animation manually.
    /// </summary>
    public void Run()
    {
        manuallyExecuted = true;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the animation name.
    /// </summary>
    private string AnimationName
        => Animation?.Name ?? string.Empty;

    /// <summary>
    /// Gets the easing name.
    /// </summary>
    private string EasingName
        => Easing?.Name ?? Easings.Ease.Name;

    /// <summary>
    /// Gets the duration value normalized as string.
    /// </summary>
    private string DurationString
        => Duration != null ? Duration.GetValueOrDefault().TotalMilliseconds.ToString( CultureInfo.InvariantCulture )
            : DurationMilliseconds != null ? DelayMilliseconds.GetValueOrDefault().ToString()
            : string.Empty;

    /// <summary>
    /// Gets the delay value normalized as string.
    /// </summary>
    private string DelayString
        => Delay != null ? Delay.GetValueOrDefault().TotalMilliseconds.ToString( CultureInfo.InvariantCulture )
            : DelayMilliseconds != null ? DelayMilliseconds.GetValueOrDefault().ToString()
            : string.Empty;

    /// <summary>
    /// Gets the mirror flag normalized as string.
    /// </summary>
    private string MirrorString
        => GetOptions()?.Mirror.ToString().ToLowerInvariant() ?? string.Empty;

    /// <summary>
    /// Gets the once flag normalized as string.
    /// </summary>
    private string OnceString
        => GetOptions()?.Once.ToString().ToLowerInvariant() ?? string.Empty;

    /// <summary>
    /// Gets or sets the reference to the rendered element.
    /// </summary>
    public ElementReference ElementRef { get; set; }

    /// <summary>
    /// Injects the globally configured <see cref="AnimateOptions"/>.
    /// </summary>
    [Inject] private IOptionsSnapshot<AnimateOptions> OptionsAccessor { get; set; }

    /// <summary>
    /// Gets or sets the animate element id.
    /// </summary>
    [Parameter] public string ElementId { get; set; }

    /// <summary>
    /// Gets or sets the animation effect.
    /// </summary>
    /// <remarks>
    /// The list of all supported animations can be found in <see cref="Animations"/> class.
    /// </remarks>
    [Parameter] public IAnimation Animation { get; set; }

    /// <summary>
    /// Gets or sets the easing effect.
    /// </summary>
    /// <remarks>
    /// The list of all supported easings can be found in <see cref="Easings"/> class.
    /// </remarks>
    [Parameter] public IEasing Easing { get; set; }

    /// <summary>
    /// Gets os sets the total duration of the animation.
    /// </summary>
    /// <remarks>
    /// Values from 0 to 3000, with step 50ms.
    /// </remarks>
    [Parameter] public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Gets os sets the total duration of the animation, in milliseconds.
    /// </summary>
    /// <remarks>
    /// Values from 0 to 3000, with step 50ms.
    /// </remarks>
    [Parameter] public int? DurationMilliseconds { get; set; }

    /// <summary>
    /// Gets os sets the delay of the animation before it runs automatically, or manually.
    /// </summary>
    /// <remarks>
    /// Values from 0 to 3000, with step 50ms.
    /// </remarks>
    [Parameter] public TimeSpan? Delay { get; set; }

    /// <summary>
    /// Gets os sets the delay in milliseconds of the animation before it runs automatically, or manually.
    /// </summary>
    /// <remarks>
    /// Values from 0 to 3000, with step 50ms.
    /// </remarks>
    [Parameter] public int? DelayMilliseconds { get; set; }

    /// <summary>
    /// Whether elements should animate out while scrolling past them.
    /// </summary>
    [Parameter] public bool? Mirror { get; set; }

    /// <summary>
    /// Whether animation should happen only once - while scrolling down.
    /// </summary>
    [Parameter] public bool? Once { get; set; }

    /// <summary>
    /// Shifts the trigger point of the animation.
    /// </summary>
    [Parameter] public int Offset { get; set; }

    /// <summary>
    /// Element whose offset will be used to trigger animation instead of an actual one.
    /// </summary>
    [Parameter] public string Anchor { get; set; }

    /// <summary>
    /// Defines which position of the element regarding to window should trigger the animation.
    /// </summary>
    [Parameter] public string AnchorPlacement { get; set; }

    /// <summary>
    /// Defines the custom name of the options to get from the configuration.
    /// </summary>
    [Parameter] public string OptionsName { get; set; } = Microsoft.Extensions.Options.Options.DefaultName;

    /// <summary>
    /// Defines the animate options.
    /// </summary>
    [Parameter] public AnimateOptions Options { get; set; }

    /// <summary>
    /// True if the animation will be executed automatically. Otherwise if false it needs to 
    /// be run manually with <see cref="Run"/> method.
    /// </summary>
    [Parameter] public bool Auto { get; set; } = true;

    /// <summary>
    /// Captures all the custom attribute that are not part of <see cref="Animate"/> component.
    /// </summary>
    [Parameter( CaptureUnmatchedValues = true )]
    public Dictionary<string, object> Attributes { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Animate"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}