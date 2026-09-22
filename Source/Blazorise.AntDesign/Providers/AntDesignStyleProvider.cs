#region Using directives
#endregion

namespace Blazorise.AntDesign.Providers;

public class AntDesignStyleProvider : StyleProvider
{
    #region Component colors

    public override string TextInputColor( Color color ) => ColorStyle( color, "--ant-input-custom-color" );

    public override string MemoInputColor( Color color ) => TextInputColor( color );

    public override string NumericInputColor( Color color ) => TextInputColor( color );

    public override string DateInputColor( Color color ) => TextInputColor( color );

    public override string TimeInputColor( Color color ) => TextInputColor( color );

    public override string DatePickerColor( Color color ) => TextInputColor( color );

    public override string TimePickerColor( Color color ) => TextInputColor( color );

    public override string NumericPickerColor( Color color ) => TextInputColor( color );

    public override string InputMaskColor( Color color ) => TextInputColor( color );

    public override string ListGroupItemColor( Color color ) => ColorStyle( color, "--ant-list-item-bg" );

    public override string TableRowColor( Color color ) => ColorStyle( color, "--ant-table-row-bg" );

    public override string TableRowCellColor( Color color ) => ColorStyle( color, "--ant-table-row-bg" );

    public override string SwitchColor( Color color ) => ColorStyle( color, "--ant-switch-bg" );

    public override string RatingItemColor( Color color ) => ColorStyle( color, "--ant-rate-star-color" );

    public override string StepItemColor( Color color ) => ColorStyle( color, "--ant-steps-item-color" );

    public override string PageProgressIndicatorColor( Color color ) => ColorStyle( color, "--ant-page-progress-bg" );

    #endregion

    #region Badge

    public override string BadgeColor( Color color ) => ColorStyle( color, "--ant-tag-default-bg" );

    #endregion

    #region Alert

    public override string AlertColor( Color color ) => ColorStyle( color, "--ant-alert-bg" );

    #endregion

    #region Button

    public override string ButtonColor( Color color ) => ColorStyle( color, "--ant-color-primary" );

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

    public override string ProgressBarColor( Color color ) => ColorStyle( color, "--ant-progress-default-color" );

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

    #region Toast

    public override string ToastAnimationDuration( bool animated, int animationDuration )
        => animated
            ? $"animation-duration: {animationDuration}ms"
            : "animation-duration: unset";

    #endregion
}