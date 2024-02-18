namespace Blazorise;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public interface IStyleProvider
{
    #region Modal

    int DefaultModalZIndex { get; }

    int DefaultModalBackdropZIndex { get; }

    string ModalShow();

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

    #region Row

    string RowGutter( (int Horizontal, int Vertical) gutter );

    #endregion

    #region Column

    string ColumnGutter( (int Horizontal, int Vertical) gutter );

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