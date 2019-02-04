#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    public class ChangingEventArgs : CancelEventArgs
    {
        public ChangingEventArgs( string oldValue, string newValue )
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public ChangingEventArgs( string oldValue, string newValue, bool cancel )
        {
            OldValue = oldValue;
            NewValue = newValue;
            Cancel = cancel;
        }

        public string OldValue { get; set; }

        public string NewValue { get; set; }
    }
}
