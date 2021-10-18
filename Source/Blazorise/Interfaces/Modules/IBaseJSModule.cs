#region Using directives
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Base contract for all JS modules.
    /// </summary>
    public interface IBaseJSModule
    {
        /// <summary>
        /// Gets the module file name.
        /// </summary>
        string ModuleFileName { get; }

        /// <summary>
        /// Gets the awaitable <see cref="IJSObjectReference"/> task.
        /// </summary>
        Task<IJSObjectReference> Module { get; }
    }
}
