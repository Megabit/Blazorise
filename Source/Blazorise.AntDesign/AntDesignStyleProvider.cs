#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.AntDesign
{
    public class AntDesignStyleProvider : IStyleProvider
    {
        #region Modal

        public virtual string ModalShow() => "display: block; padding-right: 17px;";

        #endregion

        #region ModalBody

        public virtual string ModalBodyMaxHeight( int maxHeight ) => $"max-height: {maxHeight}vh; overflow-y: auto;";

        #endregion

        #region ProgressBar

        public virtual string ProgressBarValue( int value ) => $"width: {value}%";

        #endregion

        #region Layout

        public virtual string Visibility( Visibility visibility ) => visibility == Blazorise.Visibility.Never ? "display: none;" : null;

        #endregion
    }
}
