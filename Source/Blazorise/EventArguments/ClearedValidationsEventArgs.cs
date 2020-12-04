#region Using directives
using System;
#endregion

namespace Blazorise
{
    public class ClearedValidationsEventArgs : EventArgs
    {
        public static new readonly ClearedValidationsEventArgs Empty = new ClearedValidationsEventArgs();

        public ClearedValidationsEventArgs()
        {
        }
    }
}
