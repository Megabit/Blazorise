namespace Blazorise;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public interface IStyleProvider
{
    #region Accordion

    string AccordionAnimationDuration( int? animationDuration ) => null;

    #endregion

    #region Dropdown

    string DropdownAnimationDuration( int? animationDuration ) => null;

    #endregion

    #region Carousel

    string CarouselAnimationDuration( int? animationDuration ) => null;

    #endregion

    #region Alert

    string AlertAnimationDuration( int? animationDuration ) => null;

    #endregion

    #region Collapse

    string CollapseAnimationDuration( int? animationDuration ) => null;

    #endregion

    #region MemoInput

    string MemoInputAutoSize( int minimumRows ) => $"field-sizing: content; min-block-size: {minimumRows}lh";

    #endregion

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

    #region Tooltip

    string TooltipTheme( ThemeTooltipOptions options );

    string TooltipAnchor( string anchorId );

    string TooltipShowDelay( int showDelay );

    string TooltipHideDelay( int hideDelay );

    string TooltipFadeDuration( bool fade, int fadeDuration );

    string TooltipZIndex( int? zIndex );

    #endregion

    #region Toast

    string ToastAnimationDuration( bool animated, int animationDuration );

    #endregion
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member