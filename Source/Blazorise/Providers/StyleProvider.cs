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

    #region Offcanvas

    public abstract string OffcanvasAnimationDuration( bool animated, int AnimationDuration );

    public abstract string OffcanvasBackdropAnimationDuration( bool animated, int animationDuration );

    #endregion

    #region Toast

    public abstract string ToastAnimationDuration( bool animated, int animationDuration );

    #endregion
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member