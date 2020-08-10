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
        private Select<TValue> parentSelect;
        #region Members

        #endregion

        #region Methods

        protected async virtual Task OnClickHandlerAsnc()
        {
            if ( ParentSelect != null && ParentSelect is Select<TValue> select )
            {
                await select.SelectValue( Value );
            }
        }

        #endregion

        #region Properties

        protected bool Selected => ParentSelect?.ContainsValue( Value ) == true;

        /// <summary>
        /// Convert the value to string because option tags are working with string internally. Otherwise some datatypes like booleans will not work as expected.
        /// </summary>
        [Obsolete]
        protected string StringValue => Value?.ToString();

        /// <summary>
        /// Gets or sets the item value.
        /// </summary>
        [Parameter] public TValue Value { get; set; }

        /// <summary>
        /// Disable the item from mouse click.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        protected virtual Select<TValue> ParentSelect
        {
            get => parentSelect;
            set
            {
                parentSelect = value;

                ParentSelect?.Items.TryAdd( Value, ChildContent );
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
