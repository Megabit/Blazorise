﻿#region Using directives
#endregion

namespace Blazorise.FluentUI2;

public class FluentUI2StyleProvider : StyleProvider
{
    #region Modal

    public override int DefaultModalZIndex => 40;

    public override int DefaultModalBackdropZIndex => 0;

    public override string ModalShow() => null;

    int ModalZIndexDiff => DefaultModalZIndex - DefaultModalBackdropZIndex;

    public override string ModalZIndex( int modalOpenIndex )
        => modalOpenIndex > 1 ? $"z-index: {DefaultModalZIndex + ModalZIndexDiff}" : null;

    public override string ModalBackdropZIndex( int modalOpenIndex )
        => null;

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

    #region Offcanvas

    public override string OffcanvasAnimationDuration( bool animated, int animationDuration )
        => animated
            ? $"transition-duration: {animationDuration}ms"
            : "transition-duration: unset";

    #endregion
}