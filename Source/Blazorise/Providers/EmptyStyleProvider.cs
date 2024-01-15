#region Using directives
#endregion

namespace Blazorise.Providers;

class EmptyStyleProvider : IStyleProvider
{
    #region Modal

    public int DefaultModalZIndex => 0;

    public int DefaultModalBackdropZIndex => 0;

    public string ModalShow() => null;

    public string ModalZIndex( int modalOpenIndex ) => null;

    public string ModalBackdropZIndex( int modalOpenIndex ) => null;

    #endregion

    #region ModalBody

    public string ModalBodyMaxHeight( int maxHeight ) => null;

    #endregion

    #region ProgressBar

    public string ProgressBarValue( int value ) => null;

    public string ProgressBarSize( Size size ) => null;

    #endregion

    #region Layout

    #endregion

    #region Row

    public string RowGutter( (int Horizontal, int Vertical) gutter ) => null;

    #endregion

    #region Column

    public string ColumnGutter( (int Horizontal, int Vertical) gutter ) => null;

    #endregion

    #region Offcanvas

    public string OffcanvasAnimationDuration( bool animated, int AnimationDuration ) => null;

    public string OffcanvasBackdropAnimationDuration( bool animated, int animationDuration ) => null;

    #endregion
}