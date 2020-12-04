#region Using directives
using System;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Supplies the information about the selected tab.
    /// </summary>
    public class TabsStateEventArgs : EventArgs
    {
        public TabsStateEventArgs( string tabName )
        {
            TabName = tabName;
        }

        /// <summary>
        /// Gets the selected tab name.
        /// </summary>
        public string TabName { get; }
    }
}
