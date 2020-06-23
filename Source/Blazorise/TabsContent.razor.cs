#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Stores;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class TabsContent : BaseComponent
    {
        #region Members

        private List<string> panels = new List<string>();

        private TabsContentStore store = new TabsContentStore();

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TabsContent() );

            base.BuildClasses( builder );
        }

        internal void Hook( string panelName )
        {
            panels.Add( panelName );
        }

        public void SelectPanel( string panelName )
        {
            SelectedPanel = panelName;

            StateHasChanged();
        }

        #endregion

        #region Properties

        protected TabsContentStore Store => store;

        protected int IndexOfSelectedPanel => panels.IndexOf( store.SelectedPanel );

        /// <summary>
        /// Gets or sets currently selected panel name.
        /// </summary>
        [Parameter]
        public string SelectedPanel
        {
            get => store.SelectedPanel;
            set
            {
                // prevent panels from calling the same code multiple times
                if ( value == store.SelectedPanel )
                    return;

                store.SelectedPanel = value;

                // raise the tabchanged notification
                SelectedPanelChanged.InvokeAsync( store.SelectedPanel );

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs after the selected panel has changed.
        /// </summary>
        [Parameter] public EventCallback<string> SelectedPanelChanged { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
