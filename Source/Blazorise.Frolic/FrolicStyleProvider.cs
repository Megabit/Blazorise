#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Frolic
{
    class FrolicStyleProvider : StyleProvider
    {
        #region Modal

        public override string ModalShow() => null;

        #endregion

        #region ModalBody

        public override string ModalBodyMaxHeight( int maxHeight ) => $"max-height: {maxHeight}vh; overflow-y: auto;";

        #endregion

        #region ProgressBar

        public override string ProgressBarValue( int value ) => null;

        public override string ProgressBarSize( Size size ) => null;

        #endregion

        #region Layout

        #endregion
    }
}
