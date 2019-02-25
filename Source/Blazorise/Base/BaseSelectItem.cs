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

        [Parameter] internal protected TValue Value { get; set; }

        [Parameter] protected bool IsDisabled { get; set; }

        [CascadingParameter] protected BaseSelect<TValue> ParentSelect { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
