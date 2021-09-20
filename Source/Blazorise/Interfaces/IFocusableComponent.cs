#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Default contract for the component that can be focused.
    /// </summary>
    public interface IFocusableComponent
    {
        /// <summary>
        /// Sets the focus on the underline element.
        /// </summary>
        /// <param name="scrollToElement">If true the browser should scroll the document to bring the newly-focused element into view.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Focus( bool scrollToElement = true );
    }
}
