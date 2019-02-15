#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    public delegate void ValidateEventHandler( ValidatorEventArgs e );

    public delegate void ValidationFailedEventHandler( ValidationFailedEventArgs e );

    public delegate void ValidationSucceededEventHandler( ValidationSucceededEventArgs e );

    public delegate void ValidatingEventHandler();

    public delegate void ValidatingAllEventHandler( ValidatingAllEventArgs e );

    public delegate void ValidatedAllEventHandler();

    public delegate void ClearAllValidatinaEventHandler();
}
