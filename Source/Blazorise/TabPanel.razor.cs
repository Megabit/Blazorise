#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class TabPanel : BaseComponent
    {
        #region Members

        private bool active;

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
                ParentTabs.HookPanel( Name );

                Active = Name == ParentTabs.SelectedTab;

                ParentTabs.StateChanged += OnTabsContentStateChanged;
            }

            if ( ParentTabsContent != null )
            {
                ParentTabsContent.Hook( Name );

                Active = Name == ParentTabsContent.SelectedPanel;

                ParentTabsContent.StateChanged += OnTabsContentStateChanged;
            }

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentTabs != null )
                {
                    ParentTabs.StateChanged -= OnTabsContentStateChanged;
                }

                if ( ParentTabsContent != null )
                {
                    ParentTabsContent.StateChanged -= OnTabsContentStateChanged;
                }
            }

            base.Dispose( disposing );
        }

        private void OnTabsContentStateChanged( object sender, TabsStateEventArgs e )
        {
            Active = Name == e.TabName;
        }

        private void OnTabsContentStateChanged( object sender, TabsContentStateEventArgs e )
        {
            Active = Name == e.PanelName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the panel name. Must match the coresponding tab name.
        /// </summary>
        [Parameter] public string Name { get; set; }

        /// <summary>
        /// Determines is the panel active.
        /// </summary>
        public bool Active
        {
            get => active;
            private set
            {
                active = value;

                DirtyClasses();
            }
        }

        [CascadingParameter] protected Tabs ParentTabs { get; set; }

        [CascadingParameter] protected TabsContent ParentTabsContent { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
