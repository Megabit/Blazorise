#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise
{
    public class ClassBuilder
    {
        #region Members

        private readonly StringBuilder sb = new StringBuilder();

        const char Delimiter = ' ';

        #endregion

        #region Methods

        public void Append( string value )
        {
            sb.Append( value ).Append( Delimiter );
        }

        public void Append( string value, bool condition )
        {
            if ( condition )
                sb.Append( value ).Append( Delimiter );
        }

        #endregion

        #region Properties

        public string Value => sb.ToString();

        #endregion
    }
}
