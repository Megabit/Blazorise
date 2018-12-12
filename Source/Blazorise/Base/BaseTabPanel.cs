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
    public abstract class BaseTabPanel : BaseComponent
    {
        #region Members

        private bool isActive;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.TabPanel() )
                .If( () => ClassProvider.TabPanelActive(), () => IsActive );

            base.RegisterClasses();
        }

        protected override void OnInit()
        {
            ParentTabContent?.LinkPanel( this );

            base.OnInit();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the panel name.
        /// </summary>
        [Parameter] internal protected string Name { get; set; }

        [Parameter]
        protected internal bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;

                ClassMapper.Dirty();
            }
        }

        [CascadingParameter] protected BaseTabsContent ParentTabContent { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
