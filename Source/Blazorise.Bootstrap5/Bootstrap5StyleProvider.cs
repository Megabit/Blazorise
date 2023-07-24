#region Using directives
#endregion

namespace Blazorise.Bootstrap5;

public class Bootstrap5StyleProvider : StyleProvider
{
    #region Modal

    public override int DefaultModalZIndex => 1055;

    public override int DefaultModalBackdropZIndex => 1050;

    public override string ModalShow() => "display: block;";

    int ModalZIndexDiff => DefaultModalZIndex - DefaultModalBackdropZIndex;

    public override string ModalZIndex( int modalOpenIndex )
        => modalOpenIndex > 1 ? $"z-index: {DefaultModalZIndex + ( ModalZIndexDiff * ( modalOpenIndex - 1 ) ) + ModalZIndexDiff}" : null;

    public override string ModalBackdropZIndex( int modalOpenIndex )
        => modalOpenIndex > 1 ? $"z-index: {DefaultModalZIndex + ( ModalZIndexDiff * ( modalOpenIndex - 1 ) )}" : null;

    #endregion

    #region Offcanvas

    public override int DefaultOffcanvasZindex => 1050;

    public override int DefaultOffcanvasBackdropZindex => 1040;

    public override string OffcanvasAnimationDuration( int animationDuration )
        => $"transition-duration: {animationDuration}ms";

    public override string OffcanvasBodyMaxHeight( int maxHeight )
        => maxHeight > 0 ? $"max-height: {maxHeight}px; overflow-y: auto;" : null;

    #endregion

    #region ModalBody

    public override string ModalBodyMaxHeight( int maxHeight ) => $"max-height: {maxHeight}vh; overflow-y: auto";

    #endregion

    #region ProgressBar

    public override string ProgressBarValue( int value ) => $"width: {value}%";

    public override string ProgressBarSize( Size size ) => null;

    #endregion

    #region Layout

    #endregion
}