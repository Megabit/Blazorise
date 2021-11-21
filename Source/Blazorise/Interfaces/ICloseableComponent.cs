#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Providers the necessary logic to open or close a component
    /// </summary>
    public interface ICloseableComponent
    {
        /// <summary>
        /// Opens the component.
        /// </summary>
        /// <returns></returns>
        public Task Open();

        /// <summary>
        /// Closes the component.
        /// </summary>
        /// <returns></returns>
        public Task Close();
    }
}
