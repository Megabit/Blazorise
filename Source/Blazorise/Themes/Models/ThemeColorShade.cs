#region Using directives
#endregion

namespace Blazorise
{
    public class ThemeColorShade
    {
        #region Constructors

        public ThemeColorShade( string key, string name, string value )
        {
            Key = key;
            Name = name;
            Value = value;
        }

        #endregion

        #region Methods

        #endregion

        #region Properties

        public string Key { get; }

        public string Name { get; }

        public string Value { get; }

        #endregion
    }
}
