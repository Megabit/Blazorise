#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    public delegate void ValidateEventHandler( ValidateEventArgs e );

    public delegate void ValidationFailedEventHandler( ValidationFailedEventArgs e );

    public delegate void ValidationSucceededEventHandler( ValidationSucceededEventArgs e );

    public delegate void ManualValidationEventHandler();
}
