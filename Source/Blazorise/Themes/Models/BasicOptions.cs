#region Using directives
#endregion

using System;

namespace Blazorise
{
    public class BasicOptions
    {
        public string BorderRadius { get; set; } = ".25rem";

        public override bool Equals( object obj )
        {
            return obj is BasicOptions options &&
                     BorderRadius == options.BorderRadius;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( BorderRadius );
        }

        public virtual bool HasOptions()
        {
            return !string.IsNullOrEmpty( BorderRadius );
        }
    }
}
