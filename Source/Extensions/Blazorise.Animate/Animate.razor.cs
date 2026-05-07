#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Animate;

/// <summary>
/// Runs animations for any content places inside of <see cref="Animate"/>. component.
/// </summary>
public partial class Animate : BaseComponent, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// Flag that indicates if animation was manually executed.
    /// </summary>
    private bool manuallyExecuted;

    /// <summary>
    /// Flag that indicates if the component has been initialized.
    /// </summary>
    private bool initialized;

    /// <summary>
    /// Flag that indicates if the child content should be rendered.
    /// </summary>
    private bool shouldRenderElement;

    /// <summary>
    /// Flag that indicates if an animation should run after rendering.
    /// </summary>
    private bool pendingAnimation;

    /// <summary>
    /// Stores the last visible state.
    /// </summary>
    private bool lastVisible;

    /// <summary>
    /// Stores the animation direction for the next pending animation.
    /// </summary>
    private string animationDirection = "in";

    /// <summary>
    /// Captured Mirror parameter snapshot.
    /// </summary>
    private ComponentParameterInfo<bool> paramMirror;

    /// <summary>
    /// Captured Once parameter snapshot.
    /// </summary>
    private ComponentParameterInfo<bool> paramOnce;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        JSModule ??= new JSAnimateModule( JSRuntime, VersionProvider, BlazoriseOptions );

        return base.OnInitializedAsync();
    }

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( Mirror, out paramMirror );
        parameters.TryGetParameter( Once, out paramOnce );

        return base.SetParametersAsync( parameters );
    }

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

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        if ( !initialized )
        {
            initialized = true;
            lastVisible = Visible;
            shouldRenderElement = ( Auto || manuallyExecuted ) && Visible;
            pendingAnimation = shouldRenderElement && AnimateOnInitialRender;
            animationDirection = "in";

            base.OnParametersSet();

            return;
        }

        if ( Visible != lastVisible )
        {
            lastVisible = Visible;
            animationDirection = Visible ? "in" : "out";
            pendingAnimation = Auto || manuallyExecuted;

            if ( Visible )
            {
                shouldRenderElement = Auto || manuallyExecuted;
            }
        }

        base.OnParametersSet();
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( ShouldRenderElement && pendingAnimation )
        {
            pendingAnimation = false;

            bool completed = await AnimateElement();

            if ( completed && animationDirection == "out" && Layout == AnimationLayout.None )
            {
                shouldRenderElement = false;

                await InvokeAsync( StateHasChanged );
            }
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <summary>
    /// Runs the animation manually.
    /// </summary>
    public void Run()
    {
        manuallyExecuted = true;
        shouldRenderElement = true;
        pendingAnimation = true;
        animationDirection = "in";

        StateHasChanged();
    }

    /// <summary>
    /// Runs the animation for the rendered element.
    /// </summary>
    private async Task<bool> AnimateElement()
    {
        try
        {
            return await JSModule.Animate( ElementRef, AnimationOptions );
        }
        catch ( JSException )
        {
        }
        catch ( InvalidOperationException )
        {
        }

        return true;
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && JSModule is not null )
        {
            try
            {
                await JSModule.DisposeElement( ElementRef );
                await JSModule.SafeDisposeAsync();
            }
            catch ( JSDisconnectedException )
            {
            }
            catch ( JSException )
            {
            }
            catch ( InvalidOperationException )
            {
            }

            JSModule = null;
        }

        await base.DisposeAsync( disposing );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the animation name.
    /// </summary>
    private string AnimationName
        => Animation?.Name ?? GetOptions()?.Animation?.Name ?? string.Empty;

    /// <summary>
    /// Gets the easing name.
    /// </summary>
    private string EasingName
        => Easing?.Name ?? GetOptions()?.Easing?.Name ?? Easings.Ease.Name;

    /// <summary>
    /// Gets the duration value normalized as milliseconds.
    /// </summary>
    private int? DurationMillisecondsValue
        => Duration is not null ? Convert.ToInt32( Duration.GetValueOrDefault().TotalMilliseconds )
            : DurationMilliseconds is not null ? DurationMilliseconds.GetValueOrDefault()
            : GetOptions()?.Duration > TimeSpan.Zero ? Convert.ToInt32( GetOptions().Duration.TotalMilliseconds )
            : null;

    /// <summary>
    /// Gets the delay value normalized as milliseconds.
    /// </summary>
    private int? DelayMillisecondsValue
        => Delay is not null ? Convert.ToInt32( Delay.GetValueOrDefault().TotalMilliseconds )
            : DelayMilliseconds is not null ? DelayMilliseconds.GetValueOrDefault()
            : GetOptions()?.Delay > TimeSpan.Zero ? Convert.ToInt32( GetOptions().Delay.TotalMilliseconds )
            : null;

    /// <summary>
    /// Gets the mirror flag.
    /// </summary>
    private bool MirrorValue
        => paramMirror.GetValueOrDefault( GetOptions()?.Mirror ?? false );

    /// <summary>
    /// Gets the once flag.
    /// </summary>
    private bool OnceValue
        => paramOnce.GetValueOrDefault( GetOptions()?.Once ?? false );

    /// <summary>
    /// Gets the animation options to pass to the JavaScript runtime.
    /// </summary>
    private object AnimationOptions
        => new
        {
            animation = AnimationName,
            easing = EasingName,
            duration = DurationMillisecondsValue,
            delay = DelayMillisecondsValue,
            mirror = MirrorValue,
            once = OnceValue,
            offset = Offset,
            anchor = Anchor,
            anchorPlacement = AnchorPlacement,
            trigger = Trigger.ToString(),
            direction = animationDirection,
            waitForCompletion = animationDirection == "out",
            layout = Layout.ToString()
        };

    /// <summary>
    /// Gets a flag that indicates if the child content should be rendered.
    /// </summary>
    private bool ShouldRenderElement
        => shouldRenderElement;

    /// <summary>
    /// Injects the globally configured <see cref="AnimateOptions"/>.
    /// </summary>
    [Inject] private IOptionsSnapshot<AnimateOptions> OptionsAccessor { get; set; }

    /// <summary>
    /// Injects a javascript runtime.
    /// </summary>
    [Inject] private IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// Gets or sets the version provider.
    /// </summary>
    [Inject] private IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Gets or sets the blazorise options.
    /// </summary>
    [Inject] private BlazoriseOptions BlazoriseOptions { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="JSAnimateModule"/> instance.
    /// </summary>
    protected JSAnimateModule JSModule { get; private set; }

    /// <summary>
    /// Specifies the animation effect.
    /// </summary>
    /// <remarks>
    /// The list of all supported animations can be found in <see cref="Animations"/> class.
    /// </remarks>
    [Parameter] public IAnimation Animation { get; set; }

    /// <summary>
    /// Specifies the easing effect.
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
    [Parameter] public bool Mirror { get; set; }

    /// <summary>
    /// Whether animation should happen only once - while scrolling down.
    /// </summary>
    [Parameter] public bool Once { get; set; }

    /// <summary>
    /// Shifts the trigger point of the animation.
    /// </summary>
    [Parameter] public int Offset { get; set; }

    /// <summary>
    /// Element whose offset will be used to trigger animation instead of an actual one.
    /// </summary>
    [Parameter] public string Anchor { get; set; }

    /// <summary>
    /// Specifies which position of the element regarding to window should trigger the animation.
    /// </summary>
    [Parameter] public string AnchorPlacement { get; set; }

    /// <summary>
    /// Specifies when the animation should run.
    /// </summary>
    [Parameter] public AnimationTrigger Trigger { get; set; } = AnimationTrigger.InView;

    /// <summary>
    /// Specifies whether the content is visible.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Specifies whether the animation should run on the first render.
    /// </summary>
    [Parameter] public bool AnimateOnInitialRender { get; set; } = true;

    /// <summary>
    /// Specifies which layout dimension should be animated together with the configured animation.
    /// </summary>
    [Parameter] public AnimationLayout Layout { get; set; } = AnimationLayout.None;

    /// <summary>
    /// Specifies the custom name of the options to get from the configuration.
    /// </summary>
    [Parameter] public string OptionsName { get; set; } = Microsoft.Extensions.Options.Options.DefaultName;

    /// <summary>
    /// Specifies the animate options.
    /// </summary>
    [Parameter] public AnimateOptions Options { get; set; }

    /// <summary>
    /// True if the animation will be executed automatically. Otherwise if false it needs to
    /// be run manually with <see cref="Run"/> method.
    /// </summary>
    [Parameter] public bool Auto { get; set; } = true;

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Animate"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}