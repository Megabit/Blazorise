#region Using directives
#endregion

namespace Blazorise
{
    public class ThemeInputOptions : BasicOptions
    {
        public string Color { get; set; }

        public string CheckColor { get; set; }

        public string SliderColor { get; set; }

        public override bool HasOptions()
        {
            return !string.IsNullOrEmpty( Color )
                || !string.IsNullOrEmpty( CheckColor )
                || !string.IsNullOrEmpty( SliderColor )
                || base.HasOptions();
        }
    }
}
