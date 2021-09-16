#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Default contract for the component that can be select its text.
    /// </summary>
    public interface ISelectableComponent
    {
        /// <summary>
        /// Select all text in the underline component.
        /// </summary>
        /// <param name="focus">If true, the input will be focused before the text is selected.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Select( bool focus = true );
    }
}
