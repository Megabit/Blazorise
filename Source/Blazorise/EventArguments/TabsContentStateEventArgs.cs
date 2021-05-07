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
        /// <summary>
        /// A default <see cref="TabsContentStateEventArgs"/> constructor.
        /// </summary>
        /// <param name="panelName">Panel name.</param>
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
