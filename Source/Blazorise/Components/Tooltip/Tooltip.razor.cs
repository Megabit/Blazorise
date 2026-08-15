#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Tooltips display informative text when users hover over, focus on, or tap an element.
/// </summary>
public partial class Tooltip : BaseComponent, IAsyncDisposable
{
    #region Members

    private TooltipPlacement placement = TooltipPlacement.Top;

    private bool multiline;

    private bool alwaysActive;

    private bool showArrow = true;

    private bool inline;

    private bool fade;

    private int fadeDuration = 300;

    private TooltipTrigger trigger = TooltipTrigger.MouseEnterFocus;

    private int? zIndex;

    private bool interactive;

    private int? showDelay;

    private int? hideDelay;

    private bool clickActive;

    private string anchorId;

    private string triggerTargetId;

    private bool fadeDurationDefined;

    private bool triggerTargetMouseActive;

    private bool triggerTargetFocusActive;

    private string subscribedTriggerTargetId;

    private TooltipTrigger subscribedTrigger;

    private IAsyncDisposable triggerTargetSubscription;

    private IAsyncDisposable outsideClickSubscription;

    private IAsyncDisposable keyDownSubscription;

    private Theme theme;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="Tooltip"/> constructor.
    /// </summary>
    public Tooltip()
    {
        TooltipContentClassBuilder = new( BuildTooltipContentClasses );
        TooltipSurfaceClassBuilder = new( BuildTooltipSurfaceClasses );
        TooltipArrowClassBuilder = new( BuildTooltipArrowClasses );
        TooltipSurfaceStyleBuilder = new( BuildTooltipSurfaceStyles );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        bool newFadeDurationDefined = parameters.TryGetValue<int>( nameof( FadeDuration ), out _ );

        if ( fadeDurationDefined != newFadeDurationDefined )
        {
            fadeDurationDefined = newFadeDurationDefined;
            DirtyStyles();
        }

        await base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        anchorId = IdGenerator.Generate;

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await SynchronizeTriggerTargetSubscriptions();

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
            await DisposeTriggerTargetSubscriptions();

        await base.DisposeAsync( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Tooltip() );
        builder.Append( ClassProvider.TooltipPlacement( Placement ) );
        builder.Append( ClassProvider.TooltipMultiline( Multiline ) );
        builder.Append( ClassProvider.TooltipAlwaysActive( AlwaysActive ) );
        builder.Append( ClassProvider.TooltipInline( Inline ) );
        builder.Append( ClassProvider.TooltipFade( Fade ) );

        base.BuildClasses( builder );

        AppendWrapperUtilities( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( Theme?.TooltipOptions is not null )
            builder.Append( StyleProvider.TooltipTheme( Theme.TooltipOptions ) );

        builder.Append( StyleProvider.TooltipAnchor( AnchorId ) );
        builder.Append( StyleProvider.TooltipShowDelay( EffectiveShowDelay ) );
        builder.Append( StyleProvider.TooltipHideDelay( EffectiveHideDelay ) );
        builder.Append( StyleProvider.TooltipFadeDuration( Fade, FadeDuration ), ShouldAppendFadeDuration );
        builder.Append( StyleProvider.TooltipZIndex( ZIndex ) );

        base.BuildStyles( builder );

        AppendWrapperUtilities( builder );
    }

    /// <inheritdoc/>
    protected override void BuildUtilityClasses( ClassBuilder builder, UtilityTarget target )
    {
        if ( target == UtilityTarget.Wrapper )
            base.BuildUtilityClasses( builder, target );
    }

    /// <inheritdoc/>
    protected override void BuildUtilityStyles( StyleBuilder builder, UtilityTarget target )
    {
        if ( target == UtilityTarget.Wrapper )
            base.BuildUtilityStyles( builder, target );
    }

    /// <summary>
    /// Builds the classnames for the tooltip content element.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    private void BuildTooltipContentClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TooltipContent() );
    }

    /// <summary>
    /// Builds the classnames for the tooltip surface element.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    private void BuildTooltipSurfaceClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TooltipSurface() );
        base.BuildUtilityClasses( builder, UtilityTarget.Self );
    }

    /// <summary>
    /// Builds the styles for the tooltip surface element.
    /// </summary>
    /// <param name="builder">Style builder used to append the styles.</param>
    private void BuildTooltipSurfaceStyles( StyleBuilder builder )
    {
        base.BuildUtilityStyles( builder, UtilityTarget.Self );
    }

    /// <summary>
    /// Builds the classnames for the tooltip arrow element.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    private void BuildTooltipArrowClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TooltipArrow() );
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        TooltipContentClassBuilder.Dirty();
        TooltipSurfaceClassBuilder.Dirty();
        TooltipArrowClassBuilder.Dirty();

        base.DirtyClasses();
    }

    /// <inheritdoc/>
    protected internal override void DirtyStyles()
    {
        TooltipSurfaceStyleBuilder.Dirty();

        base.DirtyStyles();
    }

    private void HandleClick()
    {
        if ( string.IsNullOrWhiteSpace( TriggerTargetId )
             && Trigger is ( TooltipTrigger.Click or TooltipTrigger.MouseEnterClick ) )
            clickActive = !clickActive;
    }

    private void HandleKeyDown( KeyboardEventArgs eventArgs )
    {
        if ( eventArgs.Key == "Escape" )
            clickActive = false;
    }

    private async Task HandleTriggerTargetEvent( DocumentEventArgs eventArgs )
    {
        bool activeChanged = false;

        switch ( eventArgs.Type )
        {
            case DocumentEventType.MouseEnter:
                activeChanged = !triggerTargetMouseActive;
                triggerTargetMouseActive = true;
                break;
            case DocumentEventType.MouseLeave:
                activeChanged = triggerTargetMouseActive;
                triggerTargetMouseActive = false;
                break;
            case DocumentEventType.FocusIn:
                activeChanged = !triggerTargetFocusActive;
                triggerTargetFocusActive = true;
                break;
            case DocumentEventType.FocusOut:
                activeChanged = triggerTargetFocusActive;
                triggerTargetFocusActive = false;
                break;
            case DocumentEventType.Click:
                clickActive = !clickActive;
                activeChanged = true;
                break;
        }

        if ( activeChanged )
            await InvokeAsync( StateHasChanged );
    }

    private async Task HandleOutsideClick( DocumentEventArgs eventArgs )
    {
        if ( !clickActive )
            return;

        clickActive = false;

        await InvokeAsync( StateHasChanged );
    }

    private async Task HandleDocumentKeyDown( DocumentEventArgs eventArgs )
    {
        if ( !clickActive )
            return;

        clickActive = false;

        await InvokeAsync( StateHasChanged );
    }

    private async Task SynchronizeTriggerTargetSubscriptions()
    {
        string triggerTargetId = string.IsNullOrWhiteSpace( TriggerTargetId ) ? null : TriggerTargetId;

        if ( subscribedTriggerTargetId == triggerTargetId && subscribedTrigger == Trigger )
            return;

        await DisposeTriggerTargetSubscriptions();

        subscribedTriggerTargetId = triggerTargetId;
        subscribedTrigger = Trigger;
        ResetTriggerTargetState();

        if ( triggerTargetId is null )
            return;

        string targetSelector = CssSelectorUtilities.BuildElementIdSelector( triggerTargetId );
        DocumentEventTypes targetEventTypes = Trigger switch
        {
            TooltipTrigger.Click => DocumentEventTypes.Click,
            TooltipTrigger.Focus => DocumentEventTypes.FocusIn | DocumentEventTypes.FocusOut,
            TooltipTrigger.MouseEnterClick => DocumentEventTypes.MouseEnter | DocumentEventTypes.MouseLeave | DocumentEventTypes.Click,
            _ => DocumentEventTypes.MouseEnter | DocumentEventTypes.MouseLeave | DocumentEventTypes.FocusIn | DocumentEventTypes.FocusOut,
        };

        triggerTargetSubscription = await DocumentObserver.Subscribe( new()
        {
            OwnerId = ElementId,
            EventTypes = targetEventTypes,
            Selector = targetSelector,
            Handler = HandleTriggerTargetEvent,
        } );

        if ( Trigger is TooltipTrigger.Click or TooltipTrigger.MouseEnterClick )
        {
            string rootSelector = CssSelectorUtilities.BuildElementIdSelector( ElementId );

            outsideClickSubscription = await DocumentObserver.Subscribe( new()
            {
                OwnerId = ElementId,
                EventTypes = DocumentEventTypes.Click,
                ExcludeSelector = $":is({targetSelector}, {rootSelector})",
                Priority = -100,
                Handler = HandleOutsideClick,
            } );

            keyDownSubscription = await DocumentObserver.Subscribe( new()
            {
                OwnerId = ElementId,
                EventTypes = DocumentEventTypes.KeyDown,
                KeysFilter = new[] { "Escape" },
                Capture = false,
                Handler = HandleDocumentKeyDown,
            } );
        }
    }

    private async ValueTask DisposeTriggerTargetSubscriptions()
    {
        if ( triggerTargetSubscription is not null )
        {
            await triggerTargetSubscription.DisposeAsync();
            triggerTargetSubscription = null;
        }

        if ( outsideClickSubscription is not null )
        {
            await outsideClickSubscription.DisposeAsync();
            outsideClickSubscription = null;
        }

        if ( keyDownSubscription is not null )
        {
            await keyDownSubscription.DisposeAsync();
            keyDownSubscription = null;
        }
    }

    private void ResetTriggerTargetState()
    {
        triggerTargetMouseActive = false;
        triggerTargetFocusActive = false;
        clickActive = false;
    }

    private static string ToTriggerName( TooltipTrigger trigger )
    {
        return trigger switch
        {
            TooltipTrigger.Click => "click",
            TooltipTrigger.Focus => "focus",
            TooltipTrigger.MouseEnterClick => "mouse-enter-click",
            _ => "mouse-enter-focus",
        };
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Tooltip content element class builder.
    /// </summary>
    protected ClassBuilder TooltipContentClassBuilder { get; private set; }

    /// <summary>
    /// Tooltip surface element class builder.
    /// </summary>
    protected ClassBuilder TooltipSurfaceClassBuilder { get; private set; }

    /// <summary>
    /// Tooltip arrow element class builder.
    /// </summary>
    protected ClassBuilder TooltipArrowClassBuilder { get; private set; }

    /// <summary>
    /// Tooltip surface element style builder.
    /// </summary>
    protected StyleBuilder TooltipSurfaceStyleBuilder { get; private set; }

    /// <summary>
    /// Gets the classnames for the tooltip content element.
    /// </summary>
    protected string TooltipContentClassNames => TooltipContentClassBuilder.Class;

    /// <summary>
    /// Gets the classnames for the tooltip surface element.
    /// </summary>
    protected string TooltipSurfaceClassNames => TooltipSurfaceClassBuilder.Class;

    /// <summary>
    /// Gets the classnames for the tooltip arrow element.
    /// </summary>
    protected string TooltipArrowClassNames => TooltipArrowClassBuilder.Class;

    /// <summary>
    /// Gets the styles for the tooltip surface element.
    /// </summary>
    protected string TooltipSurfaceStyleNames => TooltipSurfaceStyleBuilder.Styles;

    private string AnchorId => anchorId ?? ElementId;

    private string TooltipElementId => HasText ? $"{ElementId}-content" : null;

    private bool HasText => !string.IsNullOrEmpty( Text );

    private int EffectiveShowDelay => ( ShowDelay ?? Options?.TooltipOptions?.ShowDelay ) ?? 0;

    private int EffectiveHideDelay => ( HideDelay ?? Options?.TooltipOptions?.HideDelay ) ?? 0;

    private bool ShouldAppendFadeDuration => !Fade || fadeDurationDefined || Theme?.TooltipOptions is null;

    private string PlacementName => ClassProvider.ToTooltipPlacement( Placement );

    private string TriggerName => string.IsNullOrWhiteSpace( TriggerTargetId ) ? ToTriggerName( Trigger ) : "manual";

    private string IsActiveValue => ( AlwaysActive || clickActive || triggerTargetMouseActive || triggerTargetFocusActive ) ? "true" : "false";

    private string InlineValue => Inline ? "true" : "false";

    private string InteractiveValue => Interactive ? "true" : "false";

    private string MultilineValue => Multiline ? "true" : "false";

    /// <summary>
    /// Holds the information about the Blazorise global options.
    /// </summary>
    [Inject] protected BlazoriseOptions Options { get; set; }

    /// <summary>
    /// Gets the shared document observer used when the trigger is outside of this component.
    /// </summary>
    [Inject] protected IDocumentObserver DocumentObserver { get; set; }

    /// <summary>
    /// Specifies a regular tooltip's content.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Specifies the tooltip location relative to its component.
    /// </summary>
    [Parameter]
    public TooltipPlacement Placement
    {
        get => placement;
        set
        {
            if ( placement == value )
                return;

            placement = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Force the multiline display.
    /// </summary>
    [Parameter]
    public bool Multiline
    {
        get => multiline;
        set
        {
            if ( multiline == value )
                return;

            multiline = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Always show tooltip, instead of just when hovering over the element.
    /// </summary>
    [Parameter]
    public bool AlwaysActive
    {
        get => alwaysActive;
        set
        {
            if ( alwaysActive == value )
                return;

            alwaysActive = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the tooltip arrow visibility.
    /// </summary>
    [Parameter]
    public bool ShowArrow
    {
        get => showArrow;
        set
        {
            if ( showArrow == value )
                return;

            showArrow = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Force inline block instead of trying to detect the element block.
    /// </summary>
    [Parameter]
    public bool Inline
    {
        get => inline;
        set
        {
            if ( inline == value )
                return;

            inline = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Makes the tooltip fade transition.
    /// </summary>
    [Parameter]
    public bool Fade
    {
        get => fade;
        set
        {
            if ( fade == value )
                return;

            fade = value;

            DirtyClasses();
            DirtyStyles();
        }
    }

    /// <summary>
    /// Duration in ms of the fade transition animation.
    /// </summary>
    [Parameter]
    public int FadeDuration
    {
        get => fadeDuration;
        set
        {
            if ( fadeDuration == value )
                return;

            fadeDuration = value;

            DirtyStyles();
        }
    }

    /// <summary>
    /// Determines the events that cause the tooltip to show.
    /// </summary>
    [Parameter]
    public TooltipTrigger Trigger
    {
        get => trigger;
        set
        {
            if ( trigger == value )
                return;

            trigger = value;
            ResetTriggerTargetState();

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the id of an external element that triggers the tooltip.
    /// </summary>
    [Parameter]
    public string TriggerTargetId
    {
        get => triggerTargetId;
        set
        {
            if ( triggerTargetId == value )
                return;

            triggerTargetId = value;

            ResetTriggerTargetState();
        }
    }

    /// <summary>
    /// Specifies the z-index of the tooltip surface.
    /// </summary>
    [Parameter]
    public int? ZIndex
    {
        get => zIndex;
        set
        {
            if ( zIndex == value )
                return;

            zIndex = value;

            DirtyStyles();
        }
    }

    /// <summary>
    /// Determines if the tooltip has interactive content inside of it, so that it can be hovered over and clicked inside without hiding.
    /// </summary>
    [Parameter]
    public bool Interactive
    {
        get => interactive;
        set
        {
            if ( interactive == value )
                return;

            interactive = value;

            DirtyStyles();
        }
    }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Tooltip"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Specifies the delay in ms once a trigger event is fired before a Tooltip shows.
    /// </summary>
    [Parameter]
    public int? ShowDelay
    {
        get => showDelay;
        set
        {
            if ( showDelay == value )
                return;

            showDelay = value;

            DirtyStyles();
        }
    }

    /// <summary>
    /// Specifies the delay in ms once a trigger event is fired before a Tooltip hides.
    /// </summary>
    [Parameter]
    public int? HideDelay
    {
        get => hideDelay;
        set
        {
            if ( hideDelay == value )
                return;

            hideDelay = value;

            DirtyStyles();
        }
    }

    /// <summary>
    /// Cascaded theme settings.
    /// </summary>
    [CascadingParameter]
    public Theme Theme
    {
        get => theme;
        set
        {
            theme = value;

            DirtyStyles();
        }
    }

    #endregion
}