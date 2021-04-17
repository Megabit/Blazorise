﻿namespace Blazorise
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface IStyleProvider
    {
        #region Modal

        string ModalShow();

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
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
