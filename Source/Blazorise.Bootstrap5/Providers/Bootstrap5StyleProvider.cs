#region Using directives
#endregion

namespace Blazorise.Bootstrap5.Providers;

public class Bootstrap5StyleProvider : StyleProvider
{
    #region Component colors

    public override string TextInputColor( Color color ) => ColorStyle( color, "--bs-input-border-color" );

    public override string MemoInputColor( Color color ) => TextInputColor( color );

    public override string NumericInputColor( Color color ) => TextInputColor( color );

    public override string DateInputColor( Color color ) => TextInputColor( color );

    public override string TimeInputColor( Color color ) => TextInputColor( color );

    public override string DatePickerColor( Color color ) => TextInputColor( color );

    public override string TimePickerColor( Color color ) => TextInputColor( color );

    public override string NumericPickerColor( Color color ) => TextInputColor( color );

    public override string InputMaskColor( Color color ) => TextInputColor( color );

    public override string ListGroupItemColor( Color color ) => ColorStyle( color, "--bs-list-group-bg" );

    public override string TableRowColor( Color color ) => ColorStyle( color, "--bs-table-bg" );

    public override string TableRowCellColor( Color color ) => ColorStyle( color, "--bs-table-bg" );

    public override string SwitchColor( Color color ) => ColorStyle( color, "--bs-switch-bg" );

    public override string RatingItemColor( Color color ) => ColorStyle( color, "--bs-rating-color" );

    public override string StepItemColor( Color color ) => ColorStyle( color, "--bs-step-color" );

    public override string PageProgressIndicatorColor( Color color ) => ColorStyle( color, "--bs-page-progress-bg" );

    #endregion

    #region Badge

    public override string BadgeColor( Color color ) => ColorStyle( color, "--bs-badge-bg" );

    #endregion

    #region Button

    public override string ButtonColor( Color color ) => ColorStyle( color, "--bs-btn-border-color" );

    #endregion

    #region MemoInput

    public override string MemoInputAutoSize( int minimumRows ) => $"--bs-textarea-min-block-size: {minimumRows}lh";

    #endregion

    #region Accordion

    public override string AccordionAnimationDuration( int? animationDuration )
        => animationDuration.HasValue ? $"--bs-accordion-animation-duration: {animationDuration.Value}ms" : null;

    #endregion

    #region Dropdown

    public override string DropdownAnimationDuration( int? animationDuration )
        => animationDuration.HasValue ? $"--bs-dropdown-animation-duration: {animationDuration.Value}ms" : null;

    #endregion

    #region Carousel

    public override string CarouselAnimationDuration( int? animationDuration )
        => animationDuration.HasValue ? $"--bs-carousel-animation-duration: {animationDuration.Value}ms" : null;

    #endregion

    #region Alert

    public override string AlertColor( Color color ) => ColorStyle( color, "--bs-alert-bg" );

    public override string AlertAnimationDuration( int? animationDuration )
        => animationDuration.HasValue ? $"--bs-alert-animation-duration: {animationDuration.Value}ms" : null;

    #endregion

    #region Collapse

    public override string CollapseAnimationDuration( int? animationDuration )
        => animationDuration.HasValue ? $"--bs-collapse-animation-duration: {animationDuration.Value}ms" : null;

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

    public override string ProgressBarColor( Color color ) => ColorStyle( color, "--bs-progress-bar-bg" );

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
            ( "--bs-tooltip-bg", options.BackgroundColor ),
            ( "--bs-tooltip-opacity", options.BackgroundColor is null ? null : "1" ),
            ( "--bs-tooltip-color", options.Color ),
            ( "--bs-tooltip-font-size", options.FontSize ),
            ( "--bs-tooltip-border-radius", options.BorderRadius ),
            ( "--bs-tooltip-max-width", options.MaxWidth ),
            ( "--bs-tooltip-padding", options.Padding ),
            ( "--bs-tooltip-fade-duration", options.FadeTime ),
            ( "--bs-tooltip-zindex", options.ZIndex ) );

    public override string TooltipAnchor( string anchorId )
        => $"anchor-name: --bs-tooltip-{anchorId}; anchor-scope: --bs-tooltip-{anchorId}; --bs-tooltip-anchor: --bs-tooltip-{anchorId}";

    public override string TooltipShowDelay( int showDelay ) => $"--bs-tooltip-show-delay: {showDelay}ms";

    public override string TooltipHideDelay( int hideDelay ) => $"--bs-tooltip-hide-delay: {hideDelay}ms";

    public override string TooltipFadeDuration( bool fade, int fadeDuration )
        => $"--bs-tooltip-fade-duration: {( fade ? fadeDuration : 0 )}ms";

    public override string TooltipZIndex( int? zIndex ) => zIndex.HasValue
        ? $"--bs-tooltip-zindex: {zIndex.Value}"
        : null;

    #endregion

    #region Toast

    public override string ToastAnimationDuration( bool animated, int animationDuration )
        => animated
            ? $"transition-duration: {animationDuration}ms"
            : "transition-duration: unset";

    #endregion
}