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
    public partial class BarDropdownMenu : BaseComponent
    {
        #region Members

        private bool rightAligned;

        private BarDropdownStore parentStore;

        #endregion

        #region Constructors

        public BarDropdownMenu()
        {
            ContainerClassBuilder = new ClassBuilder( BuildContainerClasses );
        }

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarDropdownMenu( ParentStore.Mode ) );
            builder.Append( ClassProvider.BarDropdownMenuVisible( ParentStore.Mode, ParentStore.Visible ) );
            builder.Append( ClassProvider.BarDropdownMenuRight( ParentStore.Mode ), RightAligned );

            base.BuildClasses( builder );
        }

        protected virtual void BuildContainerClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarDropdownMenuContainer( ParentStore.Mode ) );
            builder.Append( ClassProvider.BarDropdownMenuRight( ParentStore.Mode ), RightAligned );
        }

        internal protected override void DirtyClasses()
        {
            ContainerClassBuilder.Dirty();

            base.DirtyClasses();
        }

        #endregion

        #region Properties

        protected string ContainerClassNames => ContainerClassBuilder.Class;

        protected string VisibleString => ParentStore.Visible.ToString().ToLower();

        protected ClassBuilder ContainerClassBuilder { get; private set; }

        [Parameter]
        public bool RightAligned
        {
            get => rightAligned;
            set
            {
                rightAligned = value;

                DirtyClasses();
            }
        }

        [CascadingParameter]
        protected BarDropdownStore ParentStore
        {
            get => parentStore;
            set
            {
                if ( parentStore == value )
                    return;

                parentStore = value;

                DirtyClasses();
            }
        }

        [CascadingParameter] protected BarDropdown ParentBarDropdown { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
