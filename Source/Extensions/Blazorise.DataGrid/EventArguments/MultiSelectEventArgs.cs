#region Using directives
using System;
#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// Provides all the information about the multi select event.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class MultiSelectEventArgs<TItem> : EventArgs
    {
        /// <summary>
        /// Default constructors.
        /// </summary>
        /// <param name="item">Model that belongs to the grid row.</param>
        /// <param name="selected">Indicates if the row is selected or not.</param>
        /// <param name="shiftKey">True if the user is holding shift key.</param>
        public MultiSelectEventArgs( TItem item, bool selected, bool shiftKey )
        {
            Item = item;
            Selected = selected;
            ShiftKey = shiftKey;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public TItem Item { get; }

        /// <summary>
        /// Returns true if the row is selected.
        /// </summary>
        public bool Selected { get; }

        /// <summary>
        /// Returns true if user has ShiftClicked.
        /// </summary>
        public bool ShiftKey { get; }
    }
}