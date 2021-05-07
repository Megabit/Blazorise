#region Using directives
using System;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Supplies the information about the <see cref="Validations.ClearAll"/>. event.
    /// </summary>
    public class ClearedValidationsEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the default instance of <see cref="ClearedValidationsEventArgs"/>.
        /// </summary>
        public static new readonly ClearedValidationsEventArgs Empty = new();

        /// <summary>
        /// A default <see cref="ClearedValidationsEventArgs"/> constructor.
        /// </summary>
        public ClearedValidationsEventArgs()
        {
        }
    }
}
