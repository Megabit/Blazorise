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

        private Select<TValue> parentSelect;

        #endregion

        #region Methods

        protected Task OnClickHandler()
        {
            if ( ParentSelect != null && ParentSelect is AntDesign.Select<TValue> select )
            {
                return select.NotifySelectValueChanged( Value );
            }

            return Task.CompletedTask;
        }

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

        string SelectedString => Selected.ToString().ToLowerInvariant();

        string SelectItemClass
            => $"{ClassNames} ant-select-item ant-select-item-option {( Selected ? "ant-select-item-option-selected" : "" )} {( Active ? "ant-select-item-option-active" : "" )}";

        bool Active { get; set; }

        [CascadingParameter]
        protected Select<TValue> AntParentSelect
        {
            get => parentSelect;
            set
            {
                parentSelect = value;

                // In case of usage object generic type there can be issue to get value by integer key.
                if ( typeof( TValue ) == typeof( object ) && ( parentSelect?.Items.All( i => !i.Key.Equals( Value ) ) ?? false ) )
                {
                    if ( Value is int val )
                    {
                        parentSelect?.Items.Add( ((TValue)(object)val.ToString(), ChildContent) );

                        return;
                    }
                }

                if ( parentSelect?.Items.All( i => !i.Key.Equals( Value ) ) ?? false )
                {
                    parentSelect?.Items.Add( (Value, ChildContent) );
                }
            }
        }

        #endregion
    }
}
