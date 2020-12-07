#region Using directives
using System.ComponentModel;
#endregion

namespace Blazorise
{
    public class ValidatingAllEventArgs : CancelEventArgs
    {
        public ValidatingAllEventArgs( bool cancel )
            : base( cancel )
        {
        }
    }
}
