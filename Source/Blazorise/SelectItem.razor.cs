#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class SelectItem<TValue> : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        #endregion

        #region Properties

        protected bool Selected => ParentSelect?.ContainsValue( Value ) == true;

        /// <summary>
        /// Convert the value to string because option tags are working with string internally. Otherwise some datatypes like booleans will not work as expected.
        /// </summary>
        protected string StringValue => Value?.ToString();

        /// <summary>
        /// Gets or sets the item value.
        /// </summary>
        [Parameter] public TValue Value { get; set; }

        /// <summary>
        /// Disable the item from mouse click.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        [CascadingParameter] protected virtual Select<TValue> ParentSelect { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
