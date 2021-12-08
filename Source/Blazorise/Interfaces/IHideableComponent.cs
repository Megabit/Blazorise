#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Providers the necessary logic to show or hide a component.
    /// </summary>
    public interface IHideableComponent
    {
        /// <summary>
        /// Shows the component.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task Show();

        /// <summary>
        /// Hides the component.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task Hide();
    }
}
