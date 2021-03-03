#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using Blazorise.Extensions;
#endregion

namespace Blazorise.Utilities
{
    public class ClassBuilder
    {
        #region Members

        private const char Delimiter = ' ';

        private readonly Action<ClassBuilder> buildClasses;

        private StringBuilder builder = new StringBuilder();

        private string classNames;

        private bool dirty = true;

        #endregion

        #region Constructors

        /// <summary>
        /// Default class builder constructor that accepts build action.
        /// </summary>
        /// <param name="buildStyles">Action responsible for building the classes.</param>
        public ClassBuilder( Action<ClassBuilder> buildClasses )
        {
            this.buildClasses = buildClasses;
        }

        #endregion

        #region Methods

        public void Append( string value )
        {
            if ( value == null )
                return;

            builder.Append( value ).Append( Delimiter );
        }

        public void Append( string value, bool condition )
        {
            if ( condition && value != null )
                builder.Append( value ).Append( Delimiter );
        }

        public void Append( IEnumerable<string> values )
        {
            builder.Append( string.Join( Delimiter.ToString(), values ) ).Append( Delimiter );
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
        /// Gets the class-names.
        /// </summary>
        public string Class
        {
            get
            {
                if ( dirty )
                {
                    builder = new StringBuilder();

                    buildClasses( this );

                    classNames = builder.ToString().TrimEnd()?.EmptyToNull();

                    dirty = false;
                }

                return classNames;
            }
        }

        #endregion
    }
}
