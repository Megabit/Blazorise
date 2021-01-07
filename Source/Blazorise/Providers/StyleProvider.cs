#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise
{
    public abstract class StyleProvider : IStyleProvider
    {
        #region Modal

        public abstract string ModalShow();

        #endregion

        #region ModalBody

        public abstract string ModalBodyMaxHeight( int maxHeight );

        #endregion

        #region ProgressBar

        public abstract string ProgressBarValue( int value );

        public abstract string ProgressBarSize( Size size );

        #endregion

        #region Layout

        #endregion

        #region Row

        public virtual string RowGutter( (int Horizontal, int Vertical) gutter )
        {
            var sb = new StringBuilder();

            if ( gutter.Horizontal > 0 )
                sb.Append( $"margin-left: -{gutter.Horizontal / 2}px; margin-right: -{gutter.Horizontal / 2}px;" );

            if ( gutter.Vertical > 0 )
                sb.Append( $"margin-top: -{gutter.Vertical / 2}px" );

            return sb.ToString();
        }

        #endregion

        #region Column

        public virtual string ColumnGutter( (int Horizontal, int Vertical) gutter )
        {
            var sb = new StringBuilder();

            if ( gutter.Horizontal > 0 )
                sb.Append( $"padding-left: {gutter.Horizontal / 2}px; padding-right: {gutter.Horizontal / 2}px;" );

            if ( gutter.Vertical > 0 )
                sb.Append( $"padding-top: {gutter.Vertical / 2}px; padding-bottom: {gutter.Vertical / 2}px" );

            return sb.ToString();
        }

        #endregion
    }
}
