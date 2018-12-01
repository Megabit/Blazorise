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
                    builtClass = string.Join( " ", from r in rules where r.Value() let key = r.Key() where key != null select key );

                    dirty = false;
                }

                return builtClass;
            }
        }

        #endregion
    }
}
