#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Wrapper for text, buttons, or button groups on either side of textual inputs.
    /// </summary>
    public partial class Addons : BaseComponent
    {
        #region Members

        private Size size = Size.None;

        private IFluentColumn columnSize;

        private List<Button> registeredButtons;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender && registeredButtons?.Count > 0 )
            {
                DirtyClasses();

                await InvokeAsync( StateHasChanged );
            }

            await base.OnAfterRenderAsync( firstRender );
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Addons() );
            builder.Append( ClassProvider.AddonsSize( Size ), Size != Size.None );
            builder.Append( ClassProvider.AddonsHasButton( registeredButtons?.Count > 0 ) );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Notify addons that a button is placed inside of it.
        /// </summary>
        /// <param name="button">A button reference that is placed inside of the addons.</param>
        internal void NotifyButtonInitialized( Button button )
        {
            if ( button == null )
                return;

            if ( registeredButtons == null )
                registeredButtons = new List<Button>();

            if ( !registeredButtons.Contains( button ) )
            {
                registeredButtons.Add( button );
            }
        }

        /// <summary>
        /// Notify addons that a button is removed from it.
        /// </summary>
        /// <param name="button">A button reference that is placed inside of the addons.</param>
        internal void NotifyButtonRemoved( Button button )
        {
            if ( button == null )
                return;

            if ( registeredButtons != null && registeredButtons.Contains( button ) )
            {
                registeredButtons.Remove( button );
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// True if <see cref="Addons"/> is placed inside of <see cref="Field"/> component.
        /// </summary>
        protected virtual bool ParentIsHorizontal => ParentField?.Horizontal == true;

        /// <summary>
        /// Determines how much space will be used by the addons inside of the grid row.
        /// </summary>
        [Parameter]
        public IFluentColumn ColumnSize
        {
            get => columnSize;
            set
            {
                columnSize = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Changes the size of the elements placed inside of this <see cref="Accordion"/>.
        /// </summary>
        [Parameter]
        public Size Size
        {
            get => size;
            set
            {
                size = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Accordion"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets the reference to the parent <see cref="Field"/> component.
        /// </summary>
        [CascadingParameter] protected Field ParentField { get; set; }

        #endregion
    }
}
