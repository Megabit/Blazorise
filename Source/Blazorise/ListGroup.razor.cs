#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class ListGroup : BaseComponent
    {
        #region Members

        private bool flush;

        private readonly List<ListGroupItem> childItems = new List<ListGroupItem>();

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ListGroup() );
            builder.Append( ClassProvider.ListGroupFlush(), Flush );

            base.BuildClasses( builder );
        }

        internal void LinkItem( ListGroupItem listGroupItem )
        {
            childItems.Add( listGroupItem );
        }

        public void SelectItem( string name )
        {
            foreach ( var child in childItems )
            {
                // TODO: clean this to not use child attributes directly
                child.Active = child.Name == name;
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
        public bool Flush
        {
            get => flush;
            set
            {
                flush = value;

                DirtyClasses();
            }
        }

        [Parameter] public Action<string> SelectedItemChanged { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
