#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Providers
{
    class EmptyStyleProvider : IStyleProvider
    {
        #region Modal

        public string ModalShow() => null;

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
    }
}
