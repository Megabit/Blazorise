#region Using directives
#endregion

namespace Blazorise
{
    public class BasicOptions
    {
        public string BorderRadius { get; set; } = ".25rem";

        public virtual bool HasOptions()
        {
            return !string.IsNullOrEmpty( BorderRadius );
        }
    }
}
