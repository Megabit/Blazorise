#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class TabsContent : BaseComponent
    {
        #region Members

        private string selectedPanel;

        public event EventHandler<TabsContentStateEventArgs> StateChanged;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TabsContent() );

            base.BuildClasses( builder );
        }

        public void SelectPanel( string panelName )
        {
            SelectedPanel = panelName;

            StateHasChanged();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets currently selected panel name.
        /// </summary>
        [Parameter]
        public string SelectedPanel
        {
            get => selectedPanel;
            set
            {
                // prevent panels from calling the same code multiple times
                if ( value == selectedPanel )
                    return;

                selectedPanel = value;

                // raise the tabchanged notification
                StateChanged?.Invoke( this, new TabsContentStateEventArgs( selectedPanel ) );
                SelectedPanelChanged.InvokeAsync( selectedPanel );

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
