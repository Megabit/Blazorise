#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseSelectItem<TValue> : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        #endregion

        #region Properties

        protected bool IsSelected
        {
            get
            {
                if ( ParentSelect == null )
                    return false;

                if ( ParentSelect.IsMultiple )
                    return ParentSelect.SelectedValues?.Contains( Value ) == true;

                return EqualityComparer<TValue>.Default.Equals( ParentSelect.SelectedValue, Value );
            }
        }

        /// <summary>
        /// Convert the value to string because option tags are working with string internally. Otherwise some datatypes like booleans will not work as expected.
        /// </summary>
        protected string StringValue => Value?.ToString();

        /// <summary>
        /// Gets or sets the item value.
        /// </summary>
        [Parameter] internal protected TValue Value { get; set; }

        /// <summary>
        /// Disable the item from mouse click.
        /// </summary>
        [Parameter] protected bool IsDisabled { get; set; }

        [CascadingParameter] protected BaseSelect<TValue> ParentSelect { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
