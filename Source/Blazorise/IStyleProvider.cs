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

        #endregion

        #region Layout

        string Visibility( Visibility visibility );

        #endregion
    }
}
