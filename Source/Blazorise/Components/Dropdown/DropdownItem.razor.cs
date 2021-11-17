#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A menu item for the <see cref="DropdownMenu"/> component.
    /// </summary>
    public partial class DropdownItem : BaseComponent
    {
        #region Members

        private bool active;

        private bool disabled;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.DropdownItem() );
            builder.Append( ClassProvider.DropdownItemActive( Active ) );
            builder.Append( ClassProvider.DropdownItemDisabled( Disabled ) );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Handles the onclick event, if not disabled.
        /// </summary>
        /// <returns></returns>
        protected async Task ClickHandler()
        {
            if ( !Disabled )
            {
                if ( ParentDropdown is not null )
                {
                    if ( !ParentDropdown.WasJustToggled )
                        await ParentDropdown.Hide( true );
                }
                await Clicked.InvokeAsync( Value );
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Holds the item value.
        /// </summary>
        [Parameter] public object Value { get; set; }

        /// <summary>
        /// Indicate the currently active item.
        /// </summary>
        [Parameter]
        public bool Active
        {
            get => active;
            set
            {
                if ( active == value )
                    return;

                active = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Indicate the currently disabled item.
        /// </summary>
        [Parameter]
        public bool Disabled
        {
            get => disabled;
            set
            {
                if ( disabled == value )
                    return;

                disabled = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] public EventCallback<object> Clicked { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="DropdownItem"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets the reference to the parent dropdown.
        /// </summary>
        [CascadingParameter] protected Dropdown ParentDropdown { get; set; }

        #endregion
    }
}
