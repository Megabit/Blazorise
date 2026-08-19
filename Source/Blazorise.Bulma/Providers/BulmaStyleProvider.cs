#region Using directives
#endregion

namespace Blazorise.Bulma.Providers;

public class BulmaStyleProvider : StyleProvider
{
    #region Modal

    public override int DefaultModalZIndex => 40;

    public override int DefaultModalBackdropZIndex => 0;

    public override int DefaultOnScreenKeyboardZIndex => 140;

    public override string ModalShow( bool visible ) => null;

    public override string ModalFade( bool showing, bool hiding ) => null;

    public override string ModalAnimationDuration( bool animated, int animationDuration ) => animated
        ? $"--modal-animation-duration: {animationDuration}ms"
        : "--modal-animation-duration: 0ms";

    int ModalZIndexDiff => DefaultModalZIndex - DefaultModalBackdropZIndex;

    public override string ModalZIndex( int modalOpenIndex )
        => modalOpenIndex > 1 ? $"z-index: {DefaultModalZIndex + ModalZIndexDiff}" : null;

    public override string ModalBackdropZIndex( int modalOpenIndex )
        => null;

    #endregion

    #region ModalBody

    public override string ModalBodyMaxHeight( int maxHeight ) => $"max-height: {maxHeight}vh; overflow-y: auto";

    #endregion

    #region ProgressBar

    public override string ProgressBarValue( int value ) => $"width: {value}%";

    public override string ProgressBarSize( Size size ) => null;

    #endregion

    #region Layout

    #endregion

    #region Offcanvas

    public override string OffcanvasAnimationDuration( bool animated, int animationDuration )
        => animated
            ? $"transition-duration: {animationDuration}ms"
            : "transition-duration: unset";

    public override string OffcanvasBackdropAnimationDuration( bool animated, int animationDuration )
        => animated
            ? $"transition-duration: {animationDuration}ms"
            : "transition-duration: unset";

    #endregion

    #region Tooltip

    public override string TooltipTheme( ThemeTooltipOptions options )
        => BuildStyleVariables(
            ( "--bulma-tooltip-background-color", options.BackgroundColor ),
            ( "--bulma-tooltip-color", options.Color ),
            ( "--bulma-tooltip-font-size", options.FontSize ),
            ( "--bulma-tooltip-border-radius", options.BorderRadius ),
            ( "--bulma-tooltip-max-width", options.MaxWidth ),
            ( "--bulma-tooltip-padding", options.Padding ),
            ( "--bulma-tooltip-fade-duration", options.FadeTime ),
            ( "--bulma-tooltip-z-index", options.ZIndex ) );

    public override string TooltipAnchor( string anchorId )
        => $"anchor-name: --bulma-tooltip-{anchorId}; anchor-scope: --bulma-tooltip-{anchorId}; --bulma-tooltip-anchor: --bulma-tooltip-{anchorId}";

    public override string TooltipShowDelay( int showDelay ) => $"--bulma-tooltip-show-delay: {showDelay}ms";

    public override string TooltipHideDelay( int hideDelay ) => $"--bulma-tooltip-hide-delay: {hideDelay}ms";

    public override string TooltipFadeDuration( bool fade, int fadeDuration )
        => $"--bulma-tooltip-fade-duration: {( fade ? fadeDuration : 0 )}ms";

    public override string TooltipZIndex( int? zIndex ) => zIndex.HasValue
        ? $"--bulma-tooltip-z-index: {zIndex.Value}"
        : null;

    #endregion

    #region Toast

    public override string ToastAnimationDuration( bool animated, int animationDuration )
        => animated
            ? $"animation-duration: {animationDuration}ms"
            : "animation-duration: unset";

    #endregion
}