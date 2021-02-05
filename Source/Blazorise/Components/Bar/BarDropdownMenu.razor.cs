#region Using directives
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class BarDropdownMenu : BaseComponent
    {
        #region Members

        private bool rightAligned;

        private BarDropdownState parentDropdownState;

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
            builder.Append( ClassProvider.BarDropdownMenu( ParentDropdownState.Mode ) );
            builder.Append( ClassProvider.BarDropdownMenuVisible( ParentDropdownState.Mode, ParentDropdownState.Visible ) );
            builder.Append( ClassProvider.BarDropdownMenuRight( ParentDropdownState.Mode ), RightAligned );

            base.BuildClasses( builder );
        }

        protected virtual void BuildContainerClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarDropdownMenuContainer( ParentDropdownState.Mode ) );
            builder.Append( ClassProvider.BarDropdownMenuRight( ParentDropdownState.Mode ), RightAligned );
        }

        internal protected override void DirtyClasses()
        {
            ContainerClassBuilder.Dirty();

            base.DirtyClasses();
        }

        #endregion

        #region Properties

        protected string ContainerClassNames => ContainerClassBuilder.Class;

        protected string VisibleString => ParentDropdownState.Visible.ToString().ToLower();

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
        protected BarDropdownState ParentDropdownState
        {
            get => parentDropdownState;
            set
            {
                if ( parentDropdownState == value )
                    return;

                parentDropdownState = value;

                DirtyClasses();
            }
        }

        [CascadingParameter] protected BarDropdown ParentBarDropdown { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
