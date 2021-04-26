namespace Blazorise
{
    public record ThemeColorShade
    {
        #region Constructors

        public ThemeColorShade( string key, string name, string value )
        {
            Key = key;
            Name = name;
            Value = value;
        }

        #endregion

        #region Properties

        public string Key { get; }

        public string Name { get; }

        public string Value { get; }

        #endregion
    }
}
