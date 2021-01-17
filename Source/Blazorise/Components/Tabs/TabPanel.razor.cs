﻿#region Using directives
using Blazorise.Stores;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class TabPanel : BaseComponent
    {
        #region Members

        private TabsStore parentTabsStore;

        private TabsContentStore parentTabsContentStore;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TabPanel() );
            builder.Append( ClassProvider.TabPanelActive( Active ) );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            if ( ParentTabs != null )
            {
                ParentTabs.NotifyTabPanelInitialized( Name );
            }

            if ( ParentTabsContent != null )
            {
                ParentTabsContent.NotifyTabPanelInitialized( Name );
            }

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentTabs != null )
                {
                    ParentTabs.NotifyTabPanelRemoved( Name );
                }

                if ( ParentTabsContent != null )
                {
                    ParentTabsContent.NotifyTabPanelRemoved( Name );
                }
            }

            base.Dispose( disposing );
        }

        #endregion

        #region Properties

        protected bool Active => parentTabsStore.SelectedTab == Name || parentTabsContentStore.SelectedPanel == Name;

        /// <summary>
        /// Defines the panel name. Must match the coresponding tab name.
        /// </summary>
        [Parameter] public string Name { get; set; }

        [CascadingParameter]
        protected TabsStore ParentTabsStore
        {
            get => parentTabsStore;
            set
            {
                if ( parentTabsStore == value )
                    return;

                parentTabsStore = value;

                DirtyClasses();
            }
        }

        [CascadingParameter]
        protected TabsContentStore ParentTabsContentStore
        {
            get => parentTabsContentStore;
            set
            {
                if ( parentTabsContentStore == value )
                    return;

                parentTabsContentStore = value;

                DirtyClasses();
            }
        }

        [CascadingParameter] protected Tabs ParentTabs { get; set; }

        [CascadingParameter] protected TabsContent ParentTabsContent { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
