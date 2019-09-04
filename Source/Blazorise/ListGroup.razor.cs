#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseListGroup : BaseComponent
    {
        #region Members

        private bool isFlush;

        private readonly List<BaseListGroupItem> childItems = new List<BaseListGroupItem>();

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ListGroup() );
            builder.Append( ClassProvider.ListGroupFlush(), IsFlush );

            base.BuildClasses( builder );
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
        public bool IsFlush
        {
            get => isFlush;
            set
            {
                isFlush = value;

                Dirty();
            }
        }

        [Parameter] public Action<string> SelectedItemChanged { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
