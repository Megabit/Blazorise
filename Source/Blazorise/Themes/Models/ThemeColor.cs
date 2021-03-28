#region Using directives
using System;
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

        public override bool Equals( object obj )
        {
            return obj is ThemeColor color &&
                     Key == color.Key;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( Key );
        }

        #endregion

        #region Properties

        public string Key { get; }

        public string Name { get; }

        public Dictionary<string, ThemeColorShade> Shades { get; protected set; }

        #endregion

    }
}
