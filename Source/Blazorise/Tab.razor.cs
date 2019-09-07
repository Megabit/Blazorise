#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseTab : BaseComponent
    {
        #region Members

        private bool isActive;

        private string linkClassNames;

        private bool linkDirtyClasses = true;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TabItem() );
            builder.Append( ClassProvider.TabItemActive(), IsActive );

            base.BuildClasses( builder );
        }

        protected override void DirtyClasses()
        {
            linkDirtyClasses = true;

            base.DirtyClasses();
        }

        protected override void OnInitialized()
        {
            ParentTabs?.Hook( this );

            base.OnInitialized();
        }

        protected void ClickHandler()
        {
            Clicked?.Invoke();
            ParentTabs?.SelectTab( Name );
        }

        #endregion

        #region Properties

        protected string LinkClassNames
        {
            get
            {
                if ( linkDirtyClasses )
                {
                    var classBuilder = new ClassBuilder();

                    classBuilder.Append( ClassProvider.TabLink() );
                    classBuilder.Append( ClassProvider.TabLinkActive(), IsActive );

                    linkClassNames = classBuilder.Value?.TrimEnd();

                    linkDirtyClasses = false;
                }

                return linkClassNames;
            }
        }

        /// <summary>
        /// Defines the tab name.
        /// </summary>
        [Parameter] public string Name { get; set; }

        /// <summary>
        /// Sets the active tab.
        /// </summary>
        [Parameter]
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] public Action Clicked { get; set; }

        [CascadingParameter] public BaseTabs ParentTabs { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
