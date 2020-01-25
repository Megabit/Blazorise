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

        private bool isActive;

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
            builder.Append( ClassProvider.TabItemActive(), IsActive );

            base.BuildClasses( builder );
        }

        private void BuildLinkClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TabLink() );
            builder.Append( ClassProvider.TabLinkActive(), IsActive );
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
                IsActive = Name == ParentTabs.SelectedTab;

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
            IsActive = Name == e.TabName;
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
        public bool IsActive
        {
            get => isActive;
            private set
            {
                isActive = value;

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
