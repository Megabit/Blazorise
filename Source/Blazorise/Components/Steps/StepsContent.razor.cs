#region Using directives
using System.Collections.Generic;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class StepsContent : BaseComponent
    {
        #region Members

        private StepsContentState state = new();

        private readonly List<string> stepPanels = new();

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

            InvokeAsync( StateHasChanged );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the reference to the steps content state object.
        /// </summary>
        protected StepsContentState State => state;

        /// <summary>
        /// Get the index of the currently selected panel.
        /// </summary>
        protected int IndexOfSelectedPanel => stepPanels.IndexOf( state.SelectedPanel ) + 1;

        /// <summary>
        /// Gets or sets currently selected panel name.
        /// </summary>
        [Parameter]
        public string SelectedPanel
        {
            get => state.SelectedPanel;
            set
            {
                // prevent panels from calling the same code multiple times
                if ( value == state.SelectedPanel )
                    return;

                state = state with { SelectedPanel = value };

                // raise the changed notification
                SelectedPanelChanged.InvokeAsync( state.SelectedPanel );

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
