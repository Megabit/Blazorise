#region Using directives
using System.Text;
#endregion

namespace Blazorise;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public abstract class StyleProvider : IStyleProvider
{
    #region Methods

    protected static string BuildStyleVariables( params (string Name, string Value)[] variables )
    {
        var builder = new StringBuilder();

        foreach ( (string Name, string Value) variable in variables )
        {
            if ( variable.Value is null )
                continue;

            if ( builder.Length > 0 )
                builder.Append( "; " );

            builder.Append( variable.Name ).Append( ": " ).Append( variable.Value );
        }

        return builder.Length > 0 ? builder.ToString() : null;
    }

    #endregion

    #region Accordion

    public virtual string AccordionAnimationDuration( int? animationDuration ) => null;

    #endregion

    #region Dropdown

    public virtual string DropdownAnimationDuration( int? animationDuration ) => null;

    #endregion

    #region Carousel

    public virtual string CarouselAnimationDuration( int? animationDuration ) => null;

    #endregion

    #region Alert

    public virtual string AlertAnimationDuration( int? animationDuration ) => null;

    #endregion

    #region Collapse

    public virtual string CollapseAnimationDuration( int? animationDuration ) => null;

    #endregion

    #region MemoInput

    public virtual string MemoInputAutoSize( int minimumRows ) => $"field-sizing: content; min-block-size: {minimumRows}lh";

    #endregion

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

    #region Tooltip

    public abstract string TooltipTheme( ThemeTooltipOptions options );

    public abstract string TooltipAnchor( string anchorId );

    public abstract string TooltipShowDelay( int showDelay );

    public abstract string TooltipHideDelay( int hideDelay );

    public abstract string TooltipFadeDuration( bool fade, int fadeDuration );

    public abstract string TooltipZIndex( int? zIndex );

    #endregion

    #region Toast

    public abstract string ToastAnimationDuration( bool animated, int animationDuration );

    #endregion
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member