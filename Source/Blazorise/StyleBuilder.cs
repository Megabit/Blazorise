#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise
{
    public class StyleBuilder
    {
        #region Members

        const char Delimiter = ';';

        private readonly Action<StyleBuilder> buildStyles;

        private StringBuilder builder = new StringBuilder();

        private string styles;

        private bool dirty = true;

        #endregion

        #region Constructors

        /// <summary>
        /// Default style builder constructor that accepts build action.
        /// </summary>
        /// <param name="buildStyles">Action responsible for building the styles.</param>
        public StyleBuilder( Action<StyleBuilder> buildStyles )
        {
            this.buildStyles = buildStyles;
        }

        #endregion

        #region Methods

        public void Append( string value )
        {
            if ( value != null )
                builder.Append( value ).Append( Delimiter );
        }

        public void Append( string value, bool condition )
        {
            if ( condition )
                builder.Append( value ).Append( Delimiter );
        }

        /// <summary>
        /// Marks the builder as dirty to rebuild the values.
        /// </summary>
        public void Dirty()
        {
            dirty = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the styles.
        /// </summary>
        public string Styles
        {
            get
            {
                if ( dirty )
                {
                    builder = new StringBuilder();

                    buildStyles( this );

                    styles = builder.ToString().TrimEnd( ' ', Delimiter );

                    dirty = false;
                }

                return styles;
            }
        }

        #endregion
    }
}
