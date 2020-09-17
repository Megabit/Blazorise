#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Interface to be used for the "togglers" that are responsible to open/close other components(dropdown, bar, modal, etc.).
    /// </summary>
    public interface ICloseActivator
    {
        /// <summary>
        /// Gets the id of the component that has activated the close procedure.
        /// </summary>
        string ElementId { get; }

        /// <summary>
        /// Finds if the closable component is ready to be closed.
        /// </summary>
        /// <param name="elementId">Currently clicked element id.</param>
        /// <param name="closeReason">The reason for closing the component.</param>
        /// <returns>Returns true if the closable component is ready to be closed.</returns>
        Task<bool> IsSafeToClose( string elementId, CloseReason closeReason, bool isChild );

        /// <summary>
        /// Triggers the closable component to be closed.
        /// </summary>
        /// <param name="closeReason">The reason for closing the component.</param>
        Task Close( CloseReason closeReason );
    }
}
