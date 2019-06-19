#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise
{
    public class Theme
    {
        public event EventHandler<EventArgs> Changed;

        public void ThemeHasChanged()
        {
            Changed?.Invoke( this, EventArgs.Empty );
        }

        public ThemeVariants Variants { get; set; }

        public string ButtonFocusWidth { get; set; }
    }

    public class ThemeButtons
    {
        public string BorderWidth { get; set; }

        public string HoverFocusWidth { get; set; }
    }

    public class ThemeVariants
    {
        public string Primary { get; set; }

        public string Secondary { get; set; }

        public string Success { get; set; }

        public string Info { get; set; }

        public string Warning { get; set; }

        public string Danger { get; set; }

        public string Light { get; set; }

        public string Dark { get; set; }
    }
}
