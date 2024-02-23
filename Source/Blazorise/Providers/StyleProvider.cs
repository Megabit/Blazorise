#region Using directives
using System.Text;
#endregion

namespace Blazorise;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public abstract class StyleProvider : IStyleProvider
{
    #region Modal

    public abstract int DefaultModalZIndex { get; }

    public abstract int DefaultModalBackdropZIndex { get; }

    public abstract string ModalShow( bool visible );

    public abstract string ModalFade( bool showing, bool hiding );

    public abstract string ModalAnimationDuration( bool animated, int animationDuration );

    public abstract string ModalZIndex( int modalOpenIndex );

    public abstract string ModalBackdropZIndex( int modalOpenIndex );

    #endregion

    #region ModalBody

    public abstract string ModalBodyMaxHeight( int maxHeight );

    #endregion

    #region ProgressBar

    public abstract string ProgressBarValue( int value );

    public abstract string ProgressBarSize( Size size );

    #endregion

    #region Layout

    #endregion

    #region Row

    public virtual string RowGutter( (int Horizontal, int Vertical) gutter )
    {
        var sb = new StringBuilder();

        if ( gutter.Horizontal > 0 )
            sb.Append( $"margin-left: -{gutter.Horizontal / 2}px; margin-right: -{gutter.Horizontal / 2}px;" );

        if ( gutter.Vertical > 0 )
            sb.Append( $"margin-top: -{gutter.Vertical / 2}px" );

        return sb.ToString();
    }

    #endregion

    #region Column

    public virtual string ColumnGutter( (int Horizontal, int Vertical) gutter )
    {
        var sb = new StringBuilder();

        if ( gutter.Horizontal > 0 )
            sb.Append( $"padding-left: {gutter.Horizontal / 2}px; padding-right: {gutter.Horizontal / 2}px;" );

        if ( gutter.Vertical > 0 )
            sb.Append( $"padding-top: {gutter.Vertical / 2}px; padding-bottom: {gutter.Vertical / 2}px" );

        return sb.ToString();
    }

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