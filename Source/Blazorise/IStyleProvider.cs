#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
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
}
