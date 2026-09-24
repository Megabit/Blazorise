#region Using directives
#endregion

namespace Blazorise.Tailwind.Providers;

public class TailwindStyleProvider : StyleProvider
{
    #region Component colors

    public override string TextInputColor( Color color ) => color?.IsCssValue == true
        ? ColorStyle( color, "--tw-input-color" )
        : color?.Name?.Length > 0 ? $"--tw-input-color: var(--color-{color.Name}-500, var(--btw-color-{color.Name}-500))" : null;

    public override string MemoInputColor( Color color ) => TextInputColor( color );

    public override string NumericInputColor( Color color ) => TextInputColor( color );

    public override string DateInputColor( Color color ) => TextInputColor( color );

    public override string TimeInputColor( Color color ) => TextInputColor( color );

    public override string DatePickerColor( Color color ) => TextInputColor( color );

    public override string TimePickerColor( Color color ) => TextInputColor( color );

    public override string NumericPickerColor( Color color ) => TextInputColor( color );

    public override string InputMaskColor( Color color ) => TextInputColor( color );

    public override string ListGroupItemColor( Color color ) => ColorStyle( color, "--tw-list-group-bg" );

    public override string TableRowColor( Color color ) => ColorStyle( color, "--tw-table-bg" );

    public override string TableRowCellColor( Color color ) => ColorStyle( color, "--tw-table-bg" );

    public override string SwitchColor( Color color ) => ColorStyle( color, "--tw-switch-bg" );

    public override string RatingItemColor( Color color ) => ColorStyle( color, "--tw-rating-color" );

    public override string StepItemColor( Color color ) => ColorStyle( color, "--tw-step-bg" );

    public override string PageProgressIndicatorColor( Color color ) => ColorStyle( color, "--tw-page-progress-bg" );

    #endregion

    #region Badge

    public override string BadgeColor( Color color ) => ColorStyle( color, "--tw-badge-bg" );

    #endregion

    #region Button

    public override string ButtonColor( Color color ) => ColorStyle( color, "--tw-button-bg" );

    #endregion

    #region MemoInput

    public override string MemoInputAutoSize( int minimumRows ) => $"--tw-textarea-min-block-size: {minimumRows}lh";

    #endregion

    #region Accordion

    public override string AccordionAnimationDuration( int? animationDuration )
        => animationDuration.HasValue ? $"--tw-accordion-animation-duration: {animationDuration.Value}ms" : null;

    #endregion

    #region Dropdown

    public override string DropdownAnimationDuration( int? animationDuration )
        => animationDuration.HasValue ? $"--tw-dropdown-animation-duration: {animationDuration.Value}ms" : null;

    #endregion

    #region Carousel

    public override string CarouselAnimationDuration( int? animationDuration )
        => animationDuration.HasValue ? $"--tw-carousel-animation-duration: {animationDuration.Value}ms" : null;

    #endregion

    #region Alert

    public override string AlertColor( Color color ) => ColorStyle( color, "--tw-alert-bg" );

    public override string AlertAnimationDuration( int? animationDuration )
        => animationDuration.HasValue ? $"--tw-alert-animation-duration: {animationDuration.Value}ms" : null;

    #endregion

    #region Collapse

    public override string CollapseAnimationDuration( int? animationDuration )
        => animationDuration.HasValue ? $"--tw-collapse-animation-duration: {animationDuration.Value}ms" : null;

    #endregion

    #region Modal

    public override int DefaultModalZIndex => 50;

    public override int DefaultModalBackdropZIndex => 40;

    public override int DefaultOnScreenKeyboardZIndex => 150;

    public override string ModalShow( bool visible ) => visible ? "display: flex" : null;

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

    public override string ProgressBarColor( Color color ) => ColorStyle( color, "--tw-progress-bar-bg" );

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
            ( "--tw-tooltip-background-color", options.BackgroundColor ),
            ( "--tw-tooltip-color", options.Color ),
            ( "--tw-tooltip-font-size", options.FontSize ),
            ( "--tw-tooltip-border-radius", options.BorderRadius ),
            ( "--tw-tooltip-max-width", options.MaxWidth ),
            ( "--tw-tooltip-padding", options.Padding ),
            ( "--tw-tooltip-fade-duration", options.FadeTime ),
            ( "--tw-tooltip-z-index", options.ZIndex ) );

    public override string TooltipAnchor( string anchorId )
        => $"anchor-name: --tw-tooltip-{anchorId}; anchor-scope: --tw-tooltip-{anchorId}; --tw-tooltip-anchor: --tw-tooltip-{anchorId}";

    public override string TooltipShowDelay( int showDelay ) => $"--tw-tooltip-show-delay: {showDelay}ms";

    public override string TooltipHideDelay( int hideDelay ) => $"--tw-tooltip-hide-delay: {hideDelay}ms";

    public override string TooltipFadeDuration( bool fade, int fadeDuration )
        => $"--tw-tooltip-fade-duration: {( fade ? fadeDuration : 0 )}ms";

    public override string TooltipZIndex( int? zIndex ) => zIndex.HasValue
        ? $"--tw-tooltip-z-index: {zIndex.Value}"
        : null;

    #endregion

    #region Toast

    public override string ToastAnimationDuration( bool animated, int animationDuration )
        => animated
            ? $"--toast-animation-duration: {animationDuration}ms"
            : "--toast-animation-duration: unset";

    #endregion
}