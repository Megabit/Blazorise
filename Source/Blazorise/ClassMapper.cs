#region Using directives
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
                    if ( listRules == null )
                        builtClass = string.Join( " ", from r in rules where r.Value() let key = r.Key() where key != null select key );
                    else
                        builtClass = string.Join( " ", ( from r in rules where r.Value() let key = r.Key() where key != null select key ).Concat( from lr in listRules where lr.Value() from key in lr.Key() where key != null select key ) );

                    dirty = false;
                }

                return builtClass;
            }
        }

        #endregion
    }
}
