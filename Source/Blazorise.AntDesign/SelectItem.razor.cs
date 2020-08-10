#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.AntDesign
{
    public partial class SelectItem<TValue> : Blazorise.SelectItem<TValue>
    {
        #region Members

        #endregion

        #region Methods

        protected Task OnMouseOverHandler( MouseEventArgs eventArgs )
        {
            Active = true;

            return Task.CompletedTask;
        }

        protected Task OnMouseOutHandler( MouseEventArgs eventArgs )
        {
            Active = false;

            return Task.CompletedTask;
        }

        #endregion

        #region Properties

        string SelectItemClass
            => $"{ClassNames} ant-select-item ant-select-item-option {( Selected ? "ant-select-item-option-selected" : "" )} {( Active ? "ant-select-item-option-active" : "" )}";

        bool Active { get; set; }

        [CascadingParameter]protected override Blazorise.Select<TValue> ParentSelect { get => base.ParentSelect; set => base.ParentSelect = value; }

        #endregion
    }
}
