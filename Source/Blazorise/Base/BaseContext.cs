#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// Holds the Context for an Item based component.
    /// </summary>
    /// <typeparam name="TItem">Type of an item.</typeparam>
    public abstract class BaseContext<TItem>
    {
        /// <summary>
        /// Initializes BaseContext.
        /// </summary>
        /// <param name="item">Holds the Item.</param>
        public BaseContext(TItem item)
        {
            Item = item;
        }

        /// <summary>
        /// The contextual Item.
        /// </summary>
        public TItem Item { get; }
    }
}
