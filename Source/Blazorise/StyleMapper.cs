#region Using directives
using System.Linq;
#endregion

namespace Blazorise
{
    public class StyleMapper : BaseMapper
    {
        #region Members

        private string builtStyle;

        #endregion

        #region Methods

        #endregion

        #region Properties

        public string Style
        {
            get
            {
                if ( dirty )
                {
                    // combine the styles, but only the ones that have a value
                    if ( rules != null && listRules != null )
                        builtStyle = string.Join( ";", GetValidRules().Concat( GetValidListRules() ) );
                    else if ( rules != null )
                        builtStyle = string.Join( ";", GetValidRules() );
                    else if ( listRules != null )
                        builtStyle = string.Join( ";", GetValidListRules() );

                    dirty = false;
                }

                return builtStyle;
            }
        }

        #endregion
    }
}
