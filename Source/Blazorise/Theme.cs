using System;
using System.Collections.Generic;
using System.Text;

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
    }

    public class ThemeVariants
    {
        public string Primary { get; set; }

        public string Secondary { get; set; }
    }
}
