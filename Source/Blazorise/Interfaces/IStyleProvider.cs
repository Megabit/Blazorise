namespace Blazorise;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public interface IStyleProvider
{
    #region Modal

    int DefaultModalZIndex { get; }

    int DefaultModalBackdropZIndex { get; }

    int DefaultOnScreenKeyboardZIndex => DefaultModalZIndex + 100;

    string ModalShow( bool visible );

    string ModalFade( bool showing, bool hiding );

    string ModalAnimationDuration( bool animated, int animationDuration );

    string ModalZIndex( int modalOpenIndex );

    string ModalBackdropZIndex( int modalOpenIndex );

    #endregion

    #region ModalBody

    string ModalBodyMaxHeight( int maxHeight );

    #endregion

    #region Component colors

    string TextInputColor( Color color ) => null;

    string MemoInputColor( Color color ) => null;

    string NumericInputColor( Color color ) => null;

    string DateInputColor( Color color ) => null;

    string TimeInputColor( Color color ) => null;

    string DatePickerColor( Color color ) => null;

    string TimePickerColor( Color color ) => null;

    string NumericPickerColor( Color color ) => null;

    string InputMaskColor( Color color ) => null;

    string ListGroupItemColor( Color color ) => null;

    string TableRowColor( Color color ) => null;

    string TableRowCellColor( Color color ) => null;

    string SwitchColor( Color color ) => null;

    string RatingItemColor( Color color ) => null;

    string StepItemColor( Color color ) => null;

    string PageProgressIndicatorColor( Color color ) => null;

    #endregion

    #region Badge

    string BadgeColor( Color color ) => null;

    #endregion

    #region Alert

    string AlertColor( Color color ) => null;

    #endregion

    #region Button

    string ButtonColor( Color color ) => null;

    #endregion

    #region DropdownToggle

    string DropdownToggleColor( Color color ) => ButtonColor( color );

    #endregion

    #region Progress

    string ProgressColor( Color color ) => null;

    #endregion

    #region ProgressBar

    string ProgressBarColor( Color color ) => null;

    string ProgressBarValue( int value );

    string ProgressBarSize( Size size );

    #endregion

    #region Layout

    #endregion

    #region Offcanvas

    string OffcanvasAnimationDuration( bool animated, int animationDuration );

    string OffcanvasBackdropAnimationDuration( bool animated, int animationDuration );

    #endregion

    #region Toast

    string ToastAnimationDuration( bool animated, int animationDuration );

    #endregion
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member