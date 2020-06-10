#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Stores;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Tab : BaseComponent
    {
        #region Members

        private TabsStore parentTabsStore;

        private bool disabled;

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
            builder.Append( ClassProvider.TabItemDisabled( Disabled ) );

            base.BuildClasses( builder );
        }

        private void BuildLinkClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TabLink() );
            builder.Append( ClassProvider.TabLinkActive( Active ) );
            builder.Append( ClassProvider.TabLinkDisabled( Disabled ) );
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
                ParentTabs.HookTab( Name );
            }

            base.OnInitialized();
        }

        protected Task ClickHandler()
        {
            Clicked?.Invoke();
            ParentTabs?.SelectTab( Name );

            return Task.CompletedTask;
        }

        #endregion

        #region Properties

        protected ClassBuilder LinkClassBuilder { get; private set; }

        /// <summary>
        /// Gets the link class-names.
        /// </summary>
        protected string LinkClassNames => LinkClassBuilder.Class;

        protected bool Active => parentTabsStore.SelectedTab == Name;

        /// <summary>
        /// Defines the tab name. Must match the coresponding panel name.
        /// </summary>
        [Parameter] public string Name { get; set; }

        /// <summary>
        /// Determines is the tab is disabled.
        /// </summary>
        [Parameter]
        public bool Disabled
        {
            get => disabled;
            set
            {
                disabled = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] public Action Clicked { get; set; }

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

        [CascadingParameter] protected Tabs ParentTabs { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
