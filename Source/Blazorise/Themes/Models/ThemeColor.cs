#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    public class ThemeColor
    {
        #region Constructors

        public ThemeColor( string key, string name )
        {
            Key = key;
            Name = name;
        }

        #endregion

        #region Methods

        #endregion

        #region Properties

        public string Key { get; }

        public string Name { get; }

        public Dictionary<string, ThemeColorShade> Shades { get; protected set; }

        #endregion
    }
}
