#region Using directives
#endregion

namespace Blazorise;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public abstract class StyleProvider : IStyleProvider
{
    #region Modal

    public abstract int DefaultModalZIndex { get; }

    public abstract int DefaultModalBackdropZIndex { get; }

    public virtual int DefaultOnScreenKeyboardZIndex => DefaultModalZIndex + 100;

    public abstract string ModalShow( bool visible );

    public abstract string ModalFade( bool showing, bool hiding );

    public abstract string ModalAnimationDuration( bool animated, int animationDuration );

    public abstract string ModalZIndex( int modalOpenIndex );

    public abstract string ModalBackdropZIndex( int modalOpenIndex );

    #endregion

    #region ModalBody

    public abstract string ModalBodyMaxHeight( int maxHeight );

    #endregion

    #region Color

    protected static string ColorStyle( Color color, string property )
        => color?.IsCssValue == true ? $"{property}: {color.Name}" : null;

    #endregion

    #region Component colors

    public virtual string TextInputColor( Color color ) => null;

    public virtual string MemoInputColor( Color color ) => null;

    public virtual string NumericInputColor( Color color ) => null;

    public virtual string DateInputColor( Color color ) => null;

    public virtual string TimeInputColor( Color color ) => null;

    public virtual string DatePickerColor( Color color ) => null;

    public virtual string TimePickerColor( Color color ) => null;

    public virtual string NumericPickerColor( Color color ) => null;

    public virtual string InputMaskColor( Color color ) => null;

    public virtual string ListGroupItemColor( Color color ) => null;

    public virtual string TableRowColor( Color color ) => null;

    public virtual string TableRowCellColor( Color color ) => null;

    public virtual string SwitchColor( Color color ) => null;

    public virtual string RatingItemColor( Color color ) => null;

    public virtual string StepItemColor( Color color ) => null;

    public virtual string PageProgressIndicatorColor( Color color ) => null;

    #endregion

    #region Badge

    public virtual string BadgeColor( Color color ) => null;

    #endregion

    #region Alert

    public virtual string AlertColor( Color color ) => null;

    #endregion

    #region Button

    public virtual string ButtonColor( Color color ) => null;

    #endregion

    #region DropdownToggle

    public virtual string DropdownToggleColor( Color color ) => ButtonColor( color );

    #endregion

    #region Progress

    public virtual string ProgressColor( Color color ) => null;

    #endregion

    #region ProgressBar

    public virtual string ProgressBarColor( Color color ) => null;

    public abstract string ProgressBarValue( int value );

    public abstract string ProgressBarSize( Size size );

    #endregion

    #region Offcanvas

    public abstract string OffcanvasAnimationDuration( bool animated, int AnimationDuration );

    public abstract string OffcanvasBackdropAnimationDuration( bool animated, int animationDuration );

    #endregion

    #region Toast

    public abstract string ToastAnimationDuration( bool animated, int animationDuration );

    #endregion
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member