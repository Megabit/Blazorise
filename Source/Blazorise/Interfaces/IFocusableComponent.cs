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
        /// Sets the focus on the underline component.
        /// </summary>
        /// <param name="scrollToElement">If true the browser should scroll the document to bring the newly-focused element into view.</param>
        void Focus( bool scrollToElement = true );

        /// <summary>
        /// Sets the focus on the underline component.
        /// </summary>
        /// <param name="scrollToElement">If true the browser should scroll the document to bring the newly-focused element into view.</param>
        /// <returns>Returns awaitable task.</returns>
        Task FocusAsync( bool scrollToElement = true );
    }
}
