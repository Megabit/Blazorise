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

        private List<TabPanel> childPanels = new List<TabPanel>();

        private string lastSelectePanel;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TabsContent() );

            base.BuildClasses( builder );
        }

        internal void Hook( TabPanel panel )
        {
            childPanels.Add( panel );
        }

        public void SelectPanel( string panelName )
        {
            if ( lastSelectePanel != panelName )
            {
                lastSelectePanel = panelName;

                foreach ( var child in childPanels )
                {
                    child.IsActive = child.Name == panelName;
                }

                // raise the panelchanged notification
                SelectedPanelChanged?.Invoke( panelName );

                // although nothing is actually changed we need to call this anyways or otherwise the rendering will not be called
                DirtyClasses();

                StateHasChanged();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Occurs after the selected panel has changed.
        /// </summary>
        [Parameter] public Action<string> SelectedPanelChanged { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
