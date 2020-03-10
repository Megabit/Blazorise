#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Tab : BaseComponent
    {
        #region Members

        private bool active;

        #endregion

        #region Constructors

        public Tab()
        {
            LinkClassBuilder = new ClassBuilder( BuildLinkClasses );
        }

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TabItem() );
            builder.Append( ClassProvider.TabItemActive( Active ) );

            base.BuildClasses( builder );
        }

        private void BuildLinkClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TabLink() );
            builder.Append( ClassProvider.TabLinkActive( Active ) );
        }

        internal protected override void DirtyClasses()
        {
            LinkClassBuilder.Dirty();

            base.DirtyClasses();
        }

        protected override void OnInitialized()
        {
            if ( ParentTabs != null )
            {
                Active = Name == ParentTabs.SelectedTab;

                ParentTabs.StateChanged += OnTabsStateChanged;
            }

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentTabs != null )
                {
                    ParentTabs.StateChanged -= OnTabsStateChanged;
                }
            }

            base.Dispose( disposing );
        }

        protected void ClickHandler()
        {
            Clicked?.Invoke();
            ParentTabs?.SelectTab( Name );
        }

        private void OnTabsStateChanged( object sender, TabsStateEventArgs e )
        {
            Active = Name == e.TabName;
        }

        #endregion

        #region Properties

        protected ClassBuilder LinkClassBuilder { get; private set; }

        /// <summary>
        /// Gets the link class-names.
        /// </summary>
        protected string LinkClassNames => LinkClassBuilder.Class;

        /// <summary>
        /// Defines the tab name. Must match the coresponding panel name.
        /// </summary>
        [Parameter] public string Name { get; set; }

        /// <summary>
        /// Determines is the tab active.
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

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] public Action Clicked { get; set; }

        [CascadingParameter] protected Tabs ParentTabs { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
