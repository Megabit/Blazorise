#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Tabs : BaseComponent
    {
        #region Members

        private bool pills;

        private bool fullWidth;

        private bool justified;

        private string selectedTab;

        private TabPosition tabPosition = TabPosition.Top;

        public event EventHandler<TabsStateEventArgs> StateChanged;

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
            builder.Append( ClassProvider.Tabs() );
            builder.Append( ClassProvider.TabsCards(), IsCards );
            builder.Append( ClassProvider.TabsPills(), Pills );
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

        protected bool IsCards => CardHeader != null;

        protected ClassBuilder ContentClassBuilder { get; private set; }

        /// <summary>
        /// Gets the content class-names.
        /// </summary>
        protected string ContentClassNames => ContentClassBuilder.Class;

        protected int IndexOfSelectedTab => tabItems.IndexOf( selectedTab );

        protected IReadOnlyList<string> TabItems => tabItems;

        protected IReadOnlyList<string> TabPanels => tabPanels;

        /// <summary>
        /// Makes the tab items to appear as pills.
        /// </summary>
        [Parameter]
        public bool Pills
        {
            get => pills;
            set
            {
                pills = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Makes the tab items to extend the full available width.
        /// </summary>
        [Parameter]
        public bool FullWidth
        {
            get => fullWidth;
            set
            {
                fullWidth = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Makes the tab items to extend the full available width, but every item will be the same width.
        /// </summary>
        [Parameter]
        public bool Justified
        {
            get => justified;
            set
            {
                justified = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Position of tab items.
        /// </summary>
        [Parameter]
        public TabPosition TabPosition
        {
            get => tabPosition;
            set
            {
                tabPosition = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets currently selected tab name.
        /// </summary>
        [Parameter]
        public string SelectedTab
        {
            get => selectedTab;
            set
            {
                // prevent tabs from calling the same code multiple times
                if ( value == selectedTab )
                    return;

                selectedTab = value;

                // raise the tabchanged notification
                StateChanged?.Invoke( this, new TabsStateEventArgs( selectedTab ) );
                SelectedTabChanged.InvokeAsync( selectedTab );

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
