#region Using directives
#endregion

namespace Blazorise.FluentUI2.Providers;

public class FluentUI2StyleProvider : StyleProvider
{
    #region MemoInput

    public override string MemoInputAutoSize( int minimumRows ) => $"--fui-textarea-min-block-size: {minimumRows}lh";

    #endregion

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

    public override string ModalBackdropZIndex( int modalOpenIndex )
        => modalOpenIndex > 1 ? $"z-index: {DefaultModalZIndex + ( ModalZIndexDiff * ( modalOpenIndex - 1 ) )}" : null;

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
            ? $"--durationGentle: {animationDuration}ms"
            : "--durationGentle: 0ms";

    public override string OffcanvasBackdropAnimationDuration( bool animated, int animationDuration )
        => animated
            ? $"--durationGentle: {animationDuration}ms"
            : "--durationGentle: 0ms";

    #endregion

    #region Tooltip

    public override string TooltipTheme( ThemeTooltipOptions options )
        => BuildStyleVariables(
            ( "--fui-tooltip-background-color", options.BackgroundColor ),
            ( "--fui-tooltip-color", options.Color ),
            ( "--fui-tooltip-font-size", options.FontSize ),
            ( "--fui-tooltip-border-radius", options.BorderRadius ),
            ( "--fui-tooltip-max-width", options.MaxWidth ),
            ( "--fui-tooltip-padding", options.Padding ),
            ( "--fui-tooltip-fade-duration", options.FadeTime ),
            ( "--fui-tooltip-z-index", options.ZIndex ) );

    public override string TooltipAnchor( string anchorId )
        => $"anchor-name: --fui-tooltip-{anchorId}; anchor-scope: --fui-tooltip-{anchorId}; --fui-tooltip-anchor: --fui-tooltip-{anchorId}";

    public override string TooltipShowDelay( int showDelay ) => $"--fui-tooltip-show-delay: {showDelay}ms";

    public override string TooltipHideDelay( int hideDelay ) => $"--fui-tooltip-hide-delay: {hideDelay}ms";

    public override string TooltipFadeDuration( bool fade, int fadeDuration )
        => $"--fui-tooltip-fade-duration: {( fade ? fadeDuration : 0 )}ms";

    public override string TooltipZIndex( int? zIndex ) => zIndex.HasValue
        ? $"--fui-tooltip-z-index: {zIndex.Value}"
        : null;

    #endregion

    #region Toast

    public override string ToastAnimationDuration( bool animated, int animationDuration )
        => animated
            ? $"--fui-toast-animation-duration: {animationDuration}ms"
            : "--fui-toast-animation-duration: 0ms";

    #endregion
}