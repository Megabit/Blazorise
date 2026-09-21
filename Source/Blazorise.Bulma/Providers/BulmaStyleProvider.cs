#region Using directives
#endregion

namespace Blazorise.Bulma.Providers;

public class BulmaStyleProvider : StyleProvider
{
    #region Component colors

    public override string ListGroupItemColor( Color color ) => ColorStyle( color, "--bulma-list-group-item-background-color" );

    public override string TableRowColor( Color color ) => ColorStyle( color, "--bulma-table-cell-background-color" );

    public override string TableRowCellColor( Color color ) => ColorStyle( color, "--bulma-table-cell-background-color" );

    public override string SwitchColor( Color color ) => ColorStyle( color, "--bulma-switch-background-color" );

    public override string RatingItemColor( Color color ) => ColorStyle( color, "--bulma-rating-color" );

    public override string StepItemColor( Color color ) => ColorStyle( color, "--bulma-step-color" );

    public override string PageProgressIndicatorColor( Color color ) => ColorStyle( color, "--bulma-page-progress-background-color" );

    #endregion

    #region Badge

    public override string BadgeColor( Color color ) => ColorStyle( color, "--bulma-tag-background-color" );

    #endregion

    #region Alert

    public override string AlertColor( Color color ) => ColorStyle( color, "--bulma-notification-background-color" );

    #endregion

    #region Button

    public override string ButtonColor( Color color ) => ColorStyle( color, "--bulma-button-custom-color" );

    #endregion

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

    #region Progress

    public override string ProgressColor( Color color ) => ColorStyle( color, "--bulma-progress-value-background-color" );

    #endregion

    #region ProgressBar

    public override string ProgressBarColor( Color color ) => ColorStyle( color, "--bulma-progress-value-background-color" );

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
            ? $"animation-duration: {animationDuration}ms"
            : "animation-duration: unset";

    #endregion
}