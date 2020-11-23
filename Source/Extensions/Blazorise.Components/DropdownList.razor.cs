﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Components
{
    /// <summary>
    /// A component that dynamically generated dropdown menu based on the supplied data-source.
    /// </summary>
    /// <typeparam name="TItem">Type of an item filtered by the component.</typeparam>
    /// <typeparam name="TValue">Type of an SelectedValue field.</typeparam>
    public partial class DropdownList<TItem, TValue> : ComponentBase
    {
        #region Members

        /// <summary>
        /// Reference to the Dropdown component.
        /// </summary>
        protected Dropdown dropdownRef;

        /// <summary>
        /// Reference to the DropdownToggle component.
        /// </summary>
        protected DropdownToggle dropdownToggleRef;

        #endregion

        #region Methods

        protected Task HandleDropdownItemClicked( object value )
        {
            SelectedValue = Converters.ChangeType<TValue>( value );
            return SelectedValueChanged.InvokeAsync( SelectedValue );
        }

        /// <summary>
        /// Sets focus on the input element, if it can be focused.
        /// </summary>
        /// <param name="scrollToElement">If true the browser should scroll the document to bring the newly-focused element into view.</param>
        public void Focus( bool scrollToElement = true )
        {
            dropdownToggleRef.Focus( scrollToElement );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the color of toggle button.
        /// </summary>
        [Parameter] public Color Color { get; set; }

        /// <summary>
        /// Gets or sets the DropdownList data-source.
        /// </summary>
        [Parameter] public IEnumerable<TItem> Data { get; set; }

        /// <summary>
        /// Method used to get the display field from the supplied data source.
        /// </summary>
        [Parameter] public Func<TItem, string> TextField { get; set; }

        /// <summary>
        /// Method used to get the value field from the supplied data source.
        /// </summary>
        [Parameter] public Func<TItem, TValue> ValueField { get; set; }

        /// <summary>
        /// Currently selected item value.
        /// </summary>
        [Parameter] public TValue SelectedValue { get; set; }

        /// <summary>
        /// Occurs after the selected value has changed.
        /// </summary>
        [Parameter] public EventCallback<TValue> SelectedValueChanged { get; set; }

        /// <summary>
        /// Custom classname for dropdown element.
        /// </summary>
        [Parameter] public string Class { get; set; }

        /// <summary>
        /// Custom styles for dropdown element.
        /// </summary>
        [Parameter] public string Style { get; set; }

        [Parameter( CaptureUnmatchedValues = true )]
        public Dictionary<string, object> Attributes { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
