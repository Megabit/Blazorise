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
                    if ( listRules == null )
                        builtStyle = string.Join( ";", from r in rules where r.Value() let key = r.Key() where key != null select key );
                    else
                        builtStyle = string.Join( ";", ( from r in rules where r.Value() let key = r.Key() where key != null select key ).Concat( from lr in listRules where lr.Value() from key in lr.Key() where key != null select key ) );

                    dirty = false;
                }

                return builtStyle;
            }
        }

        #endregion
    }
}
