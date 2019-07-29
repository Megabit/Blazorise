#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseTabs : BaseComponent
    {
        #region Members

        private bool isPills;

        private bool isFullWidth;

        private bool isJustified;

        private bool isVertical;

        private List<BaseTab> childTabs = new List<BaseTab>();

        private string lastSelectedTab;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Tabs() )
                .If( () => ClassProvider.TabsCards(), () => IsCards )
                .If( () => ClassProvider.TabsPills(), () => IsPills )
                .If( () => ClassProvider.TabsFullWidth(), () => IsFullWidth )
                .If( () => ClassProvider.TabsJustified(), () => IsJustified )
                .If( () => ClassProvider.TabsVertical(), () => IsVertical );

            base.RegisterClasses();
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
                ClassMapper.Dirty();

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
        private bool IsPills
        {
            get => isPills;
            set
            {
                isPills = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Makes the tab items to extend the full available width.
        /// </summary>
        [Parameter]
        private bool IsFullWidth
        {
            get => isFullWidth;
            set
            {
                isFullWidth = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Makes the tab items to extend the full available width, but every item will be the same width.
        /// </summary>
        [Parameter]
        private bool IsJustified
        {
            get => isJustified;
            set
            {
                isJustified = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Stack the navigation items by changing the flex item direction.
        /// </summary>
        [Parameter]
        private bool IsVertical
        {
            get => isVertical;
            set
            {
                isVertical = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Occurs after the selected tab has changed.
        /// </summary>
        [Parameter] protected Action<string> SelectedTabChanged { get; set; }

        [CascadingParameter] protected BaseCardHeader CardHeader { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
