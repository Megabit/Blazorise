#region Using directives
#endregion

namespace Blazorise.Bootstrap5.Providers;

public class Bootstrap5StyleProvider : StyleProvider
{
    #region Component colors

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

    #region Alert

    public override string AlertColor( Color color ) => ColorStyle( color, "--bs-alert-bg" );

    #endregion

    #region Button

    public override string ButtonColor( Color color ) => ColorStyle( color, "--bs-btn-border-color" );

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

    #region Toast

    public override string ToastAnimationDuration( bool animated, int animationDuration )
        => animated
            ? $"transition-duration: {animationDuration}ms"
            : "transition-duration: unset";

    #endregion
}