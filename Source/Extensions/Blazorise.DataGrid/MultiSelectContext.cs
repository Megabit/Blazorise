#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// Context for the row multi select mode.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class MultiSelectContext<TItem>
    {
        #region Constructors

        /// <summary>
        /// Constructor for event handler.
        /// </summary>
        /// <param name="selectedChanged">Holds the event handler for <see cref="SelectedChanged"/>.</param>
        public MultiSelectContext( EventCallback<bool> selectedChanged, bool isSelected, bool isIndeterminate )
        {
            SelectedChanged = selectedChanged;
            IsSelected = isSelected;
            IsIndeterminate = isIndeterminate;
        }

        /// <summary>
        /// Constructor for event handler and a model.
        /// </summary>
        /// <param name="selectedChanged">Holds the event handler for <see cref="SelectedChanged"/>.</param>
        /// <param name="item">Model associated with the row.</param>
        public MultiSelectContext( EventCallback<bool> selectedChanged, bool isSelected, bool isIndeterminate, TItem item )
            : this( selectedChanged, isSelected, isIndeterminate )
        {
            Item = item;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current selected state.
        /// </summary>
        public bool IsSelected { get; }

        /// <summary>
        /// Gets the current indeterminate state.
        /// </summary>
        public bool IsIndeterminate { get; }

        /// <summary>
        /// Gets the event handler for selection change.
        /// </summary>
        public EventCallback<bool> SelectedChanged { get; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public TItem Item { get; }

        #endregion
    }
}
