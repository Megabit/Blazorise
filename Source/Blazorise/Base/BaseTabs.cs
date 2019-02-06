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

        internal void LinkTab( BaseTab tab )
        {
            childTabs.Add( tab );
        }

        public void SelectTab( string tabName )
        {
            foreach ( var child in childTabs )
            {
                child.IsActive = child.Name == tabName;
            }

            SelectedTabChanged?.Invoke( tabName );
            StateHasChanged();
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

        [Parameter] protected Action<string> SelectedTabChanged { get; set; }

        [CascadingParameter] protected BaseCardHeader CardHeader { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
