#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
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

        protected override void OnInitialized()
        {
            ParentTabContent?.Hook( this );

            base.OnInitialized();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the panel name.
        /// </summary>
        [Parameter] public string Name { get; set; }

        /// <summary>
        /// Sets the active panel.
        /// </summary>
        [Parameter]
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;

                ClassMapper.Dirty();
            }
        }

        [CascadingParameter] public BaseTabsContent ParentTabContent { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
