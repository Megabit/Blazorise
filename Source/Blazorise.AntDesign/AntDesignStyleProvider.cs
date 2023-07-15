#region Using directives
#endregion

namespace Blazorise.AntDesign;

public class AntDesignStyleProvider : StyleProvider
{
    #region Modal

    public override int DefaultModalZIndex => 1000;

    public override int DefaultModalBackdropZIndex => 1000;

    public override string ModalShow() => "display: block;";

    int ModalZIndexDiff => DefaultModalZIndex - DefaultModalBackdropZIndex;

    public override string ModalZIndex( int modalOpenIndex )
        => modalOpenIndex > 1 ? $"z-index: {DefaultModalZIndex + ( ModalZIndexDiff * ( modalOpenIndex - 1 ) ) + ModalZIndexDiff}" : null;

    public override string ModalBackdropZIndex( int modalOpenIndex )
        => modalOpenIndex > 1 ? $"z-index: {DefaultModalZIndex + ( ModalZIndexDiff * ( modalOpenIndex - 1 ) )}" : null;

    #endregion

    #region ModalBody

    public override string ModalBodyMaxHeight( int maxHeight ) => $"max-height: {maxHeight}vh; overflow-y: auto";

    #endregion

    #region ProgressBar

    public override string ProgressBarValue( int value ) => $"width: {value}%";

    public override string ProgressBarSize( Size size ) => null;
    //{
    //    return size switch
    //    {
    //        Size.ExtraSmall => $"height: .25rem",
    //        Size.Small => $"height: .5rem",
    //        Size.Medium => $"height: 1.25rem",
    //        Size.Large => $"height: 1.5rem",
    //        Size.ExtraLarge => $"height: 2rem",
    //        _ => $"height: 1rem",
    //    };
    //}

    #endregion

    #region Layout

    #endregion
}