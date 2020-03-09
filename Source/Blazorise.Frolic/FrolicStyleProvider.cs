#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Frolic
{
    class FrolicStyleProvider : IStyleProvider
    {
        #region Modal

        public string ModalShow() => null;

        #endregion

        #region ModalBody

        public string ModalBodyMaxHeight( int maxHeight ) => $"max-height: {maxHeight}vh; overflow-y: auto;";

        #endregion

        #region ProgressBar

        public string ProgressBarValue( int value ) => null/*$"height: {value}%"*/;

        public string ProgressBarSize( Size size ) => null;

        #endregion

        #region Layout

        public string Visibility( Visibility visibility ) => visibility == Blazorise.Visibility.Never ? "display: none;" : null;

        #endregion
    }
}
