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
    public partial class Tabs : BaseComponent
    {
        #region Members

        private TabsStore store = new TabsStore
        {
            TabPosition = TabPosition.Top,
        };

        private List<string> tabItems = new List<string>();

        private List<string> tabPanels = new List<string>();

        #endregion

        #region Constructors

        public Tabs()
        {
            ContentClassBuilder = new ClassBuilder( BuildContentClasses );
        }

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Tabs( Pills ) );
            builder.Append( ClassProvider.TabsCards(), IsCards );
            builder.Append( ClassProvider.TabsFullWidth(), FullWidth );
            builder.Append( ClassProvider.TabsJustified(), Justified );
            builder.Append( ClassProvider.TabsVertical(), TabPosition == TabPosition.Left || TabPosition == TabPosition.Right );

            base.BuildClasses( builder );
        }

        private void BuildContentClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TabsContent() );
        }

        internal void HookTab( string tabName )
        {
            tabItems.Add( tabName );
        }

        internal void HookPanel( string panelName )
        {
            tabPanels.Add( panelName );
        }

        /// <summary>
        /// Sets the active tab by the name.
        /// </summary>
        /// <param name="tabName"></param>
        public void SelectTab( string tabName )
        {
            SelectedTab = tabName;

            StateHasChanged();
        }

        #endregion

        #region Properties

        protected TabsStore Store => store;

        protected bool IsCards => CardHeader != null;

        protected ClassBuilder ContentClassBuilder { get; private set; }

        /// <summary>
        /// Gets the content class-names.
        /// </summary>
        protected string ContentClassNames => ContentClassBuilder.Class;

        protected int IndexOfSelectedTab => tabItems.IndexOf( store.SelectedTab );

        protected IReadOnlyList<string> TabItems => tabItems;

        protected IReadOnlyList<string> TabPanels => tabPanels;

        /// <summary>
        /// Makes the tab items to appear as pills.
        /// </summary>
        [Parameter]
        public bool Pills
        {
            get => store.Pills;
            set
            {
                store.Pills = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Makes the tab items to extend the full available width.
        /// </summary>
        [Parameter]
        public bool FullWidth
        {
            get => store.FullWidth;
            set
            {
                store.FullWidth = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Makes the tab items to extend the full available width, but every item will be the same width.
        /// </summary>
        [Parameter]
        public bool Justified
        {
            get => store.Justified;
            set
            {
                store.Justified = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Position of tab items.
        /// </summary>
        [Parameter]
        public TabPosition TabPosition
        {
            get => store.TabPosition;
            set
            {
                store.TabPosition = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets currently selected tab name.
        /// </summary>
        [Parameter]
        public string SelectedTab
        {
            get => store.SelectedTab;
            set
            {
                // prevent tabs from calling the same code multiple times
                if ( value == store.SelectedTab )
                    return;

                store.SelectedTab = value;

                // raise the tabchanged notification                
                SelectedTabChanged.InvokeAsync( store.SelectedTab );

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs after the selected tab has changed.
        /// </summary>
        [Parameter] public EventCallback<string> SelectedTabChanged { get; set; }

        [CascadingParameter] protected CardHeader CardHeader { get; set; }

        [Parameter] public RenderFragment Items { get; set; }

        [Parameter] public RenderFragment Content { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
