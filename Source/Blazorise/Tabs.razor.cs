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

        private bool isPills;

        private bool isFullWidth;

        private bool isJustified;

        private bool isVertical;

        private string selectedTab;

        public event EventHandler<TabsStateEventArgs> StateChanged;

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

                DirtyClasses();
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

                DirtyClasses();
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

                DirtyClasses();
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

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
