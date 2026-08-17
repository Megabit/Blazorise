namespace Blazorise.Material.Providers;

public class MaterialStyleProvider : StyleProvider
{
    #region Modal

    public override int DefaultModalZIndex => 1055;

    public override int DefaultModalBackdropZIndex => 1050;

    public override int DefaultOnScreenKeyboardZIndex => 1155;

    public override string ModalShow( bool visible ) => null;

    public override string ModalFade( bool showing, bool hiding ) => null;

    public override string ModalAnimationDuration( bool animated, int animationDuration ) => animated
        ? $"--modal-animation-duration: {animationDuration}ms"
        : "--modal-animation-duration: 0ms";

    int ModalZIndexDiff => DefaultModalZIndex - DefaultModalBackdropZIndex;

    public override string ModalZIndex( int modalOpenIndex )
        => modalOpenIndex > 1 ? $"z-index: {DefaultModalZIndex + ( ModalZIndexDiff * ( modalOpenIndex - 1 ) ) + ModalZIndexDiff}" : null;

    public override string ModalBackdropZIndex( int modalOpenIndex ) => null;

    #endregion

    #region ModalBody

    public override string ModalBodyMaxHeight( int maxHeight ) => $"max-height: {maxHeight}vh; overflow-y: auto";

    #endregion

    #region ProgressBar

    public override string ProgressBarValue( int value ) => $"width: {value}%";

    public override string ProgressBarSize( Size size ) => null;

    #endregion

    #region Offcanvas

    public override string OffcanvasAnimationDuration( bool animated, int animationDuration )
        => animated
            ? $"--offcanvas-animation-duration: {animationDuration}ms; --offcanvas-backdrop-animation-duration: {animationDuration}ms"
            : "--offcanvas-animation-duration: 0ms; --offcanvas-backdrop-animation-duration: 0ms";

    public override string OffcanvasBackdropAnimationDuration( bool animated, int animationDuration )
        => animated
            ? $"--offcanvas-backdrop-animation-duration: {animationDuration}ms"
            : "--offcanvas-backdrop-animation-duration: 0ms";

    #endregion

    #region Tooltip

    public override string TooltipTheme( ThemeTooltipOptions options )
        => BuildStyleVariables(
            ( "--mui-tooltip-background-color", options.BackgroundColor ),
            ( "--mui-tooltip-color", options.Color ),
            ( "--mui-tooltip-font-size", options.FontSize ),
            ( "--mui-tooltip-border-radius", options.BorderRadius ),
            ( "--mui-tooltip-max-width", options.MaxWidth ),
            ( "--mui-tooltip-padding", options.Padding ),
            ( "--mui-tooltip-fade-duration", options.FadeTime ),
            ( "--mui-tooltip-z-index", options.ZIndex ) );

    public override string TooltipAnchor( string anchorId )
        => $"anchor-name: --mui-tooltip-{anchorId}; anchor-scope: --mui-tooltip-{anchorId}; --mui-tooltip-anchor: --mui-tooltip-{anchorId}";

    public override string TooltipShowDelay( int showDelay ) => $"--mui-tooltip-show-delay: {showDelay}ms";

    public override string TooltipHideDelay( int hideDelay ) => $"--mui-tooltip-hide-delay: {hideDelay}ms";

    public override string TooltipFadeDuration( bool fade, int fadeDuration )
        => $"--mui-tooltip-fade-duration: {( fade ? fadeDuration : 0 )}ms";

    public override string TooltipZIndex( int? zIndex ) => zIndex.HasValue
        ? $"--mui-tooltip-z-index: {zIndex.Value}"
        : null;

    #endregion

    #region Toast

    public override string ToastAnimationDuration( bool animated, int animationDuration )
        => animated
            ? $"--mui-toast-animation-duration: {animationDuration}ms"
            : "--mui-toast-animation-duration: 0ms";

    #endregion
}