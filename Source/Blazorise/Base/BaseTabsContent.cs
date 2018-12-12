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
    public abstract class BaseTabsContent : BaseComponent
    {
        #region Members

        private List<BaseTabPanel> childPanels = new List<BaseTabPanel>();

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.TabsContent() );

            base.RegisterClasses();
        }

        internal void LinkPanel( BaseTabPanel panel )
        {
            childPanels.Add( panel );
        }

        public void SelectPanel( string panelName )
        {
            foreach ( var child in childPanels )
            {
                child.IsActive = child.Name == panelName;
            }

            SelectedPanelChanged?.Invoke( panelName );
            StateHasChanged();
        }

        #endregion

        #region Properties

        [Parameter] protected Action<string> SelectedPanelChanged { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
