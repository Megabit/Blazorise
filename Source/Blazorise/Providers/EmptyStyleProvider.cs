#region Using directives
#endregion

namespace Blazorise.Providers;

class EmptyStyleProvider : IStyleProvider
{
    #region Accordion

    public string AccordionAnimationDuration( int? animationDuration ) => null;

    #endregion

    #region Dropdown

    public string DropdownAnimationDuration( int? animationDuration ) => null;

    #endregion

    #region Carousel

    public string CarouselAnimationDuration( int? animationDuration ) => null;

    #endregion

    #region Alert

    public string AlertAnimationDuration( int? animationDuration ) => null;

    #endregion

    #region Collapse

    public string CollapseAnimationDuration( int? animationDuration ) => null;

    #endregion

    #region MemoInput

    public string MemoInputAutoSize( int minimumRows ) => $"field-sizing: content; min-block-size: {minimumRows}lh";

    #endregion

    #region Modal

    public int DefaultModalZIndex => 0;

    public int DefaultModalBackdropZIndex => 0;

    public int DefaultOnScreenKeyboardZIndex => 100;

    public string ModalShow( bool visible ) => null;

    public string ModalFade( bool showing, bool hiding ) => null;

    public string ModalAnimationDuration( bool animated, int animationDuration ) => null;

    public string ModalZIndex( int modalOpenIndex ) => null;

    public string ModalBackdropZIndex( int modalOpenIndex ) => null;

    #endregion

    #region ModalBody

    public string ModalBodyMaxHeight( int maxHeight ) => null;

    #endregion

    #region Component colors

    public string TextInputColor( Color color ) => null;

    public string MemoInputColor( Color color ) => null;

    public string NumericInputColor( Color color ) => null;

    public string DateInputColor( Color color ) => null;

    public string TimeInputColor( Color color ) => null;

    public string DatePickerColor( Color color ) => null;

    public string TimePickerColor( Color color ) => null;

    public string NumericPickerColor( Color color ) => null;

    public string InputMaskColor( Color color ) => null;

    public string ListGroupItemColor( Color color ) => null;

    public string TableRowColor( Color color ) => null;

    public string TableRowCellColor( Color color ) => null;

    public string SwitchColor( Color color ) => null;

    public string RatingItemColor( Color color ) => null;

    public string StepItemColor( Color color ) => null;

    public string PageProgressIndicatorColor( Color color ) => null;

    #endregion

    #region Badge

    public string BadgeColor( Color color ) => null;

    #endregion

    #region Alert

    public string AlertColor( Color color ) => null;

    #endregion

    #region Button

    public string ButtonColor( Color color ) => null;

    #endregion

    #region DropdownToggle

    public string DropdownToggleColor( Color color ) => ButtonColor( color );

    #endregion

    #region Progress

    public string ProgressColor( Color color ) => null;

    #endregion

    #region ProgressBar

    public string ProgressBarColor( Color color ) => null;

    public string ProgressBarValue( int value ) => null;

    public string ProgressBarSize( Size size ) => null;

    #endregion

    #region Layout

    #endregion

    #region Offcanvas

    public string OffcanvasAnimationDuration( bool animated, int AnimationDuration ) => null;

    public string OffcanvasBackdropAnimationDuration( bool animated, int animationDuration ) => null;

    #endregion

    #region Tooltip

    public string TooltipTheme( ThemeTooltipOptions options ) => null;

    public string TooltipAnchor( string anchorId ) => null;

    public string TooltipShowDelay( int showDelay ) => null;

    public string TooltipHideDelay( int hideDelay ) => null;

    public string TooltipFadeDuration( bool fade, int fadeDuration ) => null;

    public string TooltipZIndex( int? zIndex ) => null;

    #endregion

    #region Toast

    public string ToastAnimationDuration( bool animated, int animationDuration ) => null;

    #endregion
}