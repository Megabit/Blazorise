#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseTabsContent : BaseComponent
    {
        #region Members

        private List<BaseTabPanel> childPanels = new List<BaseTabPanel>();

        private string lastSelectePanel;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.TabsContent() );

            base.RegisterClasses();
        }

        internal void Hook( BaseTabPanel panel )
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
                ClassMapper.Dirty();

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
