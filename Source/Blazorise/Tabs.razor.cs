#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseTabs : BaseComponent
    {
        #region Members

        private bool isPills;

        private bool isFullWidth;

        private bool isJustified;

        private bool isVertical;

        private readonly List<BaseTab> childTabs = new List<BaseTab>();

        private string lastSelectedTab;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Tabs() );
            builder.Append( ClassProvider.TabsCards(), IsCards );
            builder.Append( ClassProvider.TabsPills(), IsPills );
            builder.Append( ClassProvider.TabsFullWidth(), IsFullWidth );
            builder.Append( ClassProvider.TabsJustified(), IsJustified );
            builder.Append( ClassProvider.TabsVertical(), IsVertical );

            base.BuildClasses( builder );
        }

        internal void Hook( BaseTab tab )
        {
            childTabs.Add( tab );
        }

        /// <summary>
        /// Sets the active tab by the name.
        /// </summary>
        /// <param name="tabName"></param>
        public void SelectTab( string tabName )
        {
            if ( lastSelectedTab != tabName )
            {
                lastSelectedTab = tabName;

                foreach ( var child in childTabs )
                {
                    child.IsActive = child.Name == tabName;
                }

                // raise the tabchanged notification
                SelectedTabChanged?.Invoke( tabName );

                // although nothing is actually changed we need to call this anyways or otherwise the rendering will not be called
                Dirty();

                StateHasChanged();
            }
        }

        #endregion

        #region Properties

        private bool IsCards => CardHeader != null;

        /// <summary>
        /// Makes the tab items to appear as pills.
        /// </summary>
        [Parameter]
        public bool IsPills
        {
            get => isPills;
            set
            {
                isPills = value;

                Dirty();
            }
        }

        /// <summary>
        /// Makes the tab items to extend the full available width.
        /// </summary>
        [Parameter]
        public bool IsFullWidth
        {
            get => isFullWidth;
            set
            {
                isFullWidth = value;

                Dirty();
            }
        }

        /// <summary>
        /// Makes the tab items to extend the full available width, but every item will be the same width.
        /// </summary>
        [Parameter]
        public bool IsJustified
        {
            get => isJustified;
            set
            {
                isJustified = value;

                Dirty();
            }
        }

        /// <summary>
        /// Stack the navigation items by changing the flex item direction.
        /// </summary>
        [Parameter]
        public bool IsVertical
        {
            get => isVertical;
            set
            {
                isVertical = value;

                Dirty();
            }
        }

        /// <summary>
        /// Occurs after the selected tab has changed.
        /// </summary>
        [Parameter] public Action<string> SelectedTabChanged { get; set; }

        [CascadingParameter] public BaseCardHeader CardHeader { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
