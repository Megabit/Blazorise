#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseListGroup : BaseComponent
    {
        #region Members

        private bool isFlush;

        private List<BaseListGroupItem> childItems = new List<BaseListGroupItem>();

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.ListGroup() )
                .If( () => ClassProvider.ListGroupFlush(), () => IsFlush );

            base.RegisterClasses();
        }

        internal void LinkItem( BaseListGroupItem listGroupItem )
        {
            childItems.Add( listGroupItem );
        }

        public void SelectItem( string name )
        {
            foreach ( var child in childItems )
            {
                child.IsActive = child.Name == name;
            }

            SelectedItemChanged?.Invoke( name );
            StateHasChanged();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Remove some borders and rounded corners to render list group items edge-to-edge in a parent container (e.g., cards).
        /// </summary>
        [Parameter]
        protected bool IsFlush
        {
            get => isFlush;
            set
            {
                isFlush = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected Action<string> SelectedItemChanged { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
