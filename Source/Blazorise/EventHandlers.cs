#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    public delegate void ValidationStartedEventHandler();

    public delegate void ValidatingAllEventHandler( ValidatingAllEventArgs e );

    public delegate void ClearAllValidationsEventHandler();

    public delegate void ValidationsStatusChangedEventHandler( ValidationsStatusChangedEventArgs e );
}
