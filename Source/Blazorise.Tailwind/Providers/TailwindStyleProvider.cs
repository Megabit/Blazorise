#region Using directives
#endregion

namespace Blazorise.Tailwind.Providers;

public class TailwindStyleProvider : StyleProvider
{
    #region Component colors

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

    #region Alert

    public override string AlertColor( Color color ) => ColorStyle( color, "--tw-alert-bg" );

    #endregion

    #region Button

    public override string ButtonColor( Color color ) => ColorStyle( color, "--tw-button-bg" );

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

    #region Toast

    public override string ToastAnimationDuration( bool animated, int animationDuration )
        => animated
            ? $"--toast-animation-duration: {animationDuration}ms"
            : "--toast-animation-duration: unset";

    #endregion
}