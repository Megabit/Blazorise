#region Using directives
#endregion

namespace Blazorise.Providers;

class EmptyStyleProvider : IStyleProvider
{
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

    #region Toast

    public string ToastAnimationDuration( bool animated, int animationDuration ) => null;

    #endregion
}