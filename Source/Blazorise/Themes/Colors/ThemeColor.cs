#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    public record ThemeColor
    {
        #region Constructors

        public ThemeColor( string key, string name )
        {
            Key = key;
            Name = name;
        }

        #endregion

        #region Properties

        public string Key { get; }

        public string Name { get; }

        public Dictionary<string, ThemeColorShade> Shades { get; protected set; }

        #endregion
    }
}
