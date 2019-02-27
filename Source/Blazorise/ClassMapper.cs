#region Using directives
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise
{
    public class ClassMapper : BaseMapper
    {
        #region Members

        private string builtClass;

        #endregion

        #region Methods

        #endregion

        #region Properties

        public string Class
        {
            get
            {
                if ( dirty )
                {
                    // combine the classes, but only the ones that have a value
                    if ( rules != null && listRules != null )
                        builtClass = string.Join( " ", GetValidRules().Concat( GetValidListRules() ) );
                    else if ( rules != null )
                        builtClass = string.Join( " ", GetValidRules() );
                    else if ( listRules != null )
                        builtClass = string.Join( " ", GetValidListRules() );
                    else
                        builtClass = null;

                    dirty = false;
                }

                return builtClass;
            }
        }

        #endregion
    }
}
