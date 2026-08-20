#region Using directives
#endregion

namespace Blazorise.AntDesign.Providers;

public class AntDesignStyleProvider : StyleProvider
{
    #region MemoInput

    public override string MemoInputAutoSize( int minimumRows ) => $"--ant-textarea-min-block-size: {minimumRows}lh";

    #endregion

    #region Modal

    public override int DefaultModalZIndex => 1000;

    public override int DefaultModalBackdropZIndex => 1000;

    public override int DefaultOnScreenKeyboardZIndex => 1100;

    public override string ModalShow( bool visible ) => visible ? "display: block" : null;

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
    //{
    //    return size switch
    //    {
    //        Size.ExtraSmall => $"height: .25rem",
    //        Size.Small => $"height: .5rem",
    //        Size.Medium => $"height: 1.25rem",
    //        Size.Large => $"height: 1.5rem",
    //        Size.ExtraLarge => $"height: 2rem",
    //        _ => $"height: 1rem",
    //    };
    //}

    #endregion

    #region Layout

    #endregion

    #region Offcanvas

    public override string OffcanvasAnimationDuration( bool animated, int animationDuration )
        => animated
            ? $"--offcanvas-animation-duration: {animationDuration}ms"
            : "--offcanvas-animation-duration: 0ms";

    public override string OffcanvasBackdropAnimationDuration( bool animated, int animationDuration )
        => animated
            ? $"--offcanvas-backdrop-animation-duration: {animationDuration}ms"
            : "--offcanvas-backdrop-animation-duration: 0ms";

    #endregion

    #region Tooltip

    public override string TooltipTheme( ThemeTooltipOptions options )
        => BuildStyleVariables(
            ( "--ant-tooltip-bg", options.BackgroundColor ),
            ( "--ant-tooltip-color", options.Color ),
            ( "--ant-tooltip-font-size", options.FontSize ),
            ( "--ant-tooltip-border-radius", options.BorderRadius ),
            ( "--ant-tooltip-max-width", options.MaxWidth ),
            ( "--ant-tooltip-padding", options.Padding ),
            ( "--ant-tooltip-fade-duration", options.FadeTime ),
            ( "--ant-tooltip-z-index", options.ZIndex ) );

    public override string TooltipAnchor( string anchorId )
        => $"anchor-name: --ant-tooltip-{anchorId}; anchor-scope: --ant-tooltip-{anchorId}; --ant-tooltip-anchor: --ant-tooltip-{anchorId}";

    public override string TooltipShowDelay( int showDelay ) => $"--ant-tooltip-show-delay: {showDelay}ms";

    public override string TooltipHideDelay( int hideDelay ) => $"--ant-tooltip-hide-delay: {hideDelay}ms";

    public override string TooltipFadeDuration( bool fade, int fadeDuration )
        => $"--ant-tooltip-fade-duration: {( fade ? fadeDuration : 0 )}ms";

    public override string TooltipZIndex( int? zIndex ) => zIndex.HasValue
        ? $"--ant-tooltip-z-index: {zIndex.Value}"
        : null;

    #endregion

    #region Toast

    public override string ToastAnimationDuration( bool animated, int animationDuration )
        => animated
            ? $"animation-duration: {animationDuration}ms"
            : "animation-duration: unset";

    #endregion
}