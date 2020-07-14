#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.AntDesign
{
    public class AntDesignStyleProvider : StyleProvider
    {
        #region Modal

        public override string ModalShow() => "display: block; padding-right: 17px;";

        #endregion

        #region ModalBody

        public override string ModalBodyMaxHeight( int maxHeight ) => $"max-height: {maxHeight}vh; overflow-y: auto;";

        #endregion

        #region ProgressBar

        public override string ProgressBarValue( int value ) => $"width: {value}%";

        public override string ProgressBarSize( Size size )
        {
            return size switch
            {
                Size.ExtraSmall => $"height: .25rem",
                Size.Small => $"height: .5rem",
                Size.Medium => $"height: 1.25rem",
                Size.Large => $"height: 1.5rem",
                Size.ExtraLarge => $"height: 2rem",
                _ => $"height: 1rem",
            };
        }

        #endregion

        #region Layout

        #endregion
    }
}
