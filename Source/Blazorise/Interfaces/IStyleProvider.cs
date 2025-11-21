namespace Blazorise;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public interface IStyleProvider
{
    #region Modal

    int DefaultModalZIndex { get; }

    int DefaultModalBackdropZIndex { get; }

    string ModalShow( bool visible );

    string ModalFade( bool showing, bool hiding );

    string ModalAnimationDuration( bool animated, int animationDuration );

    string ModalZIndex( int modalOpenIndex );

    string ModalBackdropZIndex( int modalOpenIndex );

    #endregion

    #region ModalBody

    string ModalBodyMaxHeight( int maxHeight );

    #endregion

    #region ProgressBar

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