#region Using directives
using System.Collections.Generic;
using Blazorise.Stores;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class StepsContent : BaseComponent
    {
        #region Members

        private List<string> stepPanels = new List<string>();

        private StepsContentStore store = new StepsContentStore();

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.StepsContent() );

            base.BuildClasses( builder );
        }

        internal void NotifyStepPanelInitialized( string name )
        {
            if ( !stepPanels.Contains( name ) )
                stepPanels.Add( name );
        }

        internal void NotifyStepPanelRemoved( string name )
        {
            if ( stepPanels.Contains( name ) )
                stepPanels.Remove( name );
        }

        public void SelectPanel( string name )
        {
            SelectedPanel = name;

            StateHasChanged();
        }

        #endregion

        #region Properties

        protected StepsContentStore Store => store;

        protected int IndexOfSelectedPanel => stepPanels.IndexOf( store.SelectedPanel ) + 1;

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

                store = store with { SelectedPanel = value };

                // raise the changed notification
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
