#region Using directives
using System;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Supplies the information about the selected panel.
    /// </summary>
    public class TabsContentStateEventArgs : EventArgs
    {
        public TabsContentStateEventArgs( string panelName )
        {
            PanelName = panelName;
        }

        /// <summary>
        /// Gets the selected panel name.
        /// </summary>
        public string PanelName { get; }
    }
}
