#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Extensions;
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

    private string anchorId;

    private ComponentParameterInfo<bool> paramInline;

    private ComponentParameterInfo<int> paramFadeDuration;

    private DocumentEventTypes activeTriggerEvents;

    private (string TargetId, TooltipTrigger Trigger) subscribedTriggerTarget;

    private List<IAsyncDisposable> triggerTargetSubscriptions;

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
        ComponentParameterInfo<bool> previousParamInline = paramInline;
        ComponentParameterInfo<int> previousParamFadeDuration = paramFadeDuration;

        parameters.TryGetParameter( Inline, out paramInline );
        parameters.TryGetParameter( FadeDuration, out paramFadeDuration );

        bool classesChanged = parameters.IsParameterChanged( Placement )
                              || parameters.IsParameterChanged( Multiline )
                              || parameters.IsParameterChanged( AlwaysActive )
                              || parameters.IsParameterChanged( Fade )
                              || paramInline.Changed
                              || previousParamInline.Defined != paramInline.Defined;
        bool stylesChanged = parameters.IsParameterChanged( Fade )
                             || parameters.IsParameterChanged( ZIndex )
                             || parameters.IsParameterChanged( ShowDelay )
                             || parameters.IsParameterChanged( HideDelay )
                             || paramFadeDuration.Changed
                             || previousParamFadeDuration.Defined != paramFadeDuration.Defined
                             || parameters.TryGetValue<Theme>( nameof( Theme ), out _ );
        bool triggerChanged = parameters.IsParameterChanged( Trigger )
                              || parameters.IsParameterChanged( TriggerTargetId );

        await base.SetParametersAsync( parameters );

        if ( triggerChanged )
            ResetTriggerState();

        if ( classesChanged )
            DirtyClasses();

        if ( stylesChanged )
            DirtyStyles();
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
        builder.Append( ClassProvider.TooltipInline( paramInline.GetValueOrDefault( false ) ) );
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
        builder.Append( StyleProvider.TooltipFadeDuration( Fade, EffectiveFadeDuration ), ShouldApplyFadeDuration );
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
            ToggleTriggerEvent( DocumentEventTypes.Click );
    }

    private void HandleKeyDown( KeyboardEventArgs eventArgs )
    {
        if ( eventArgs.Key == "Escape" )
            SetTriggerEventActive( DocumentEventTypes.Click, false );
    }

    private async Task HandleTriggerTargetEvent( DocumentEventArgs eventArgs )
    {
        bool activeChanged = eventArgs.Type switch
        {
            DocumentEventType.MouseEnter => SetTriggerEventActive( DocumentEventTypes.MouseEnter, true ),
            DocumentEventType.MouseLeave => SetTriggerEventActive( DocumentEventTypes.MouseEnter, false ),
            DocumentEventType.FocusIn => SetTriggerEventActive( DocumentEventTypes.FocusIn, true ),
            DocumentEventType.FocusOut => SetTriggerEventActive( DocumentEventTypes.FocusIn, false ),
            DocumentEventType.Click => ToggleTriggerEvent( DocumentEventTypes.Click ),
            _ => false,
        };

        if ( activeChanged )
            await InvokeAsync( StateHasChanged );
    }

    private async Task HandleOutsideClick( DocumentEventArgs eventArgs )
    {
        if ( !IsTriggerEventActive( DocumentEventTypes.Click ) )
            return;

        SetTriggerEventActive( DocumentEventTypes.Click, false );

        await InvokeAsync( StateHasChanged );
    }

    private async Task HandleDocumentKeyDown( DocumentEventArgs eventArgs )
    {
        if ( !IsTriggerEventActive( DocumentEventTypes.Click ) )
            return;

        SetTriggerEventActive( DocumentEventTypes.Click, false );

        await InvokeAsync( StateHasChanged );
    }

    private async Task SynchronizeTriggerTargetSubscriptions()
    {
        string triggerTargetId = string.IsNullOrWhiteSpace( TriggerTargetId ) ? null : TriggerTargetId;

        (string TargetId, TooltipTrigger Trigger) triggerTarget = (triggerTargetId, Trigger);

        if ( subscribedTriggerTarget == triggerTarget )
            return;

        await DisposeTriggerTargetSubscriptions();

        subscribedTriggerTarget = triggerTarget;
        ResetTriggerState();

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

        triggerTargetSubscriptions = new();
        triggerTargetSubscriptions.Add( await DocumentObserver.Subscribe( new()
        {
            OwnerId = ElementId,
            EventTypes = targetEventTypes,
            Selector = targetSelector,
            Handler = HandleTriggerTargetEvent,
        } ) );

        if ( Trigger is TooltipTrigger.Click or TooltipTrigger.MouseEnterClick )
        {
            string rootSelector = CssSelectorUtilities.BuildElementIdSelector( ElementId );

            triggerTargetSubscriptions.Add( await DocumentObserver.Subscribe( new()
            {
                OwnerId = ElementId,
                EventTypes = DocumentEventTypes.Click,
                ExcludeSelector = $":is({targetSelector}, {rootSelector})",
                Priority = -100,
                Handler = HandleOutsideClick,
            } ) );

            triggerTargetSubscriptions.Add( await DocumentObserver.Subscribe( new()
            {
                OwnerId = ElementId,
                EventTypes = DocumentEventTypes.KeyDown,
                KeysFilter = new[] { "Escape" },
                Capture = false,
                Handler = HandleDocumentKeyDown,
            } ) );
        }
    }

    private async ValueTask DisposeTriggerTargetSubscriptions()
    {
        if ( triggerTargetSubscriptions is null )
            return;

        foreach ( IAsyncDisposable subscription in triggerTargetSubscriptions )
            await subscription.DisposeAsync();

        triggerTargetSubscriptions = null;
    }

    private bool IsTriggerEventActive( DocumentEventTypes eventType )
    {
        return ( activeTriggerEvents & eventType ) != 0;
    }

    private bool SetTriggerEventActive( DocumentEventTypes eventType, bool active )
    {
        bool changed = IsTriggerEventActive( eventType ) != active;

        if ( active )
            activeTriggerEvents |= eventType;
        else
            activeTriggerEvents &= ~eventType;

        return changed;
    }

    private bool ToggleTriggerEvent( DocumentEventTypes eventType )
    {
        return SetTriggerEventActive( eventType, !IsTriggerEventActive( eventType ) );
    }

    private void ResetTriggerState()
    {
        activeTriggerEvents = DocumentEventTypes.None;
    }

    private static string ToTriggerString( TooltipTrigger trigger )
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

    private int EffectiveFadeDuration => paramFadeDuration.GetValueOrDefault( 300 );

    private bool ShouldApplyFadeDuration => !Fade || paramFadeDuration.Defined || Theme?.TooltipOptions is null;

    private string PlacementString => ClassProvider.ToTooltipPlacement( Placement );

    private string TriggerString => string.IsNullOrWhiteSpace( TriggerTargetId ) ? ToTriggerString( Trigger ) : "manual";

    private string ActiveString => ( AlwaysActive || activeTriggerEvents != DocumentEventTypes.None ) ? "true" : "false";

    private string InlineString => paramInline.Defined ? ( paramInline.Value ? "true" : "false" ) : "auto";

    private string InteractiveString => Interactive ? "true" : "false";

    private string MultilineString => Multiline ? "true" : "false";

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
    [Parameter] public TooltipPlacement Placement { get; set; } = TooltipPlacement.Top;

    /// <summary>
    /// Force the multiline display.
    /// </summary>
    [Parameter] public bool Multiline { get; set; }

    /// <summary>
    /// Always show tooltip, instead of just when hovering over the element.
    /// </summary>
    [Parameter] public bool AlwaysActive { get; set; }

    /// <summary>
    /// Specifies the tooltip arrow visibility.
    /// </summary>
    [Parameter] public bool ShowArrow { get; set; } = true;

    /// <summary>
    /// Forces the tooltip host to use inline-block layout.
    /// </summary>
    /// <remarks>
    /// When this parameter is not supplied, inline layout is automatically detected from the target element.
    /// Explicitly setting it to <see langword="false"/> disables automatic detection.
    /// </remarks>
    [Parameter] public bool Inline { get; set; }

    /// <summary>
    /// Makes the tooltip fade transition.
    /// </summary>
    [Parameter] public bool Fade { get; set; }

    /// <summary>
    /// Duration in ms of the fade transition animation.
    /// </summary>
    [Parameter] public int FadeDuration { get; set; } = 300;

    /// <summary>
    /// Determines the events that cause the tooltip to show.
    /// </summary>
    [Parameter] public TooltipTrigger Trigger { get; set; } = TooltipTrigger.MouseEnterFocus;

    /// <summary>
    /// Specifies the id of an external element that triggers the tooltip.
    /// </summary>
    [Parameter] public string TriggerTargetId { get; set; }

    /// <summary>
    /// Specifies the z-index of the tooltip surface.
    /// </summary>
    [Parameter] public int? ZIndex { get; set; }

    /// <summary>
    /// Determines if the tooltip has interactive content inside of it, so that it can be hovered over and clicked inside without hiding.
    /// </summary>
    [Parameter] public bool Interactive { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Tooltip"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Specifies the delay in ms once a trigger event is fired before a Tooltip shows.
    /// </summary>
    [Parameter] public int? ShowDelay { get; set; }

    /// <summary>
    /// Specifies the delay in ms once a trigger event is fired before a Tooltip hides.
    /// </summary>
    [Parameter] public int? HideDelay { get; set; }

    /// <summary>
    /// Cascaded theme settings.
    /// </summary>
    [CascadingParameter] public Theme Theme { get; set; }

    #endregion
}