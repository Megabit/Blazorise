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
            if ( ParentTabContent != null )
            {
                ParentTabContent.Hook( Name );

                Active = Name == ParentTabContent.SelectedPanel;

                ParentTabContent.StateChanged += OnTabsContentStateChanged;
            }

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentTabContent != null )
                {
                    ParentTabContent.StateChanged -= OnTabsContentStateChanged;
                }
            }

            base.Dispose( disposing );
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

        [CascadingParameter] protected TabsContent ParentTabContent { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
