#region Using directives
using System.Collections.Generic;
using Blazorise.Stores;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class TabsContent : BaseComponent
    {
        #region Members

        private List<string> tabPanels = new List<string>();

        private TabsContentStore store = new TabsContentStore();

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TabsContent() );

            base.BuildClasses( builder );
        }

        internal void NotifyTabPanelInitialized( string name )
        {
            if ( !tabPanels.Contains( name ) )
                tabPanels.Add( name );
        }

        internal void NotifyTabPanelRemoved( string name )
        {
            if ( tabPanels.Contains( name ) )
                tabPanels.Remove( name );
        }

        public void SelectPanel( string name )
        {
            SelectedPanel = name;

            StateHasChanged();
        }

        #endregion

        #region Properties

        protected TabsContentStore Store => store;

        protected int IndexOfSelectedPanel => tabPanels.IndexOf( store.SelectedPanel );

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
