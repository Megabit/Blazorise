#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Container for any type of icon font.
    /// </summary>
    public partial class Icon : BaseComponent
    {
        #region Members

        private object name;

        private IconStyle iconStyle = IconStyle.Solid;

        private IconSize iconSize = IconSize.Default;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( IconProvider.Icon( Name, IconStyle ) );
            builder.Append( IconProvider.IconSize( IconSize ), IconSize != IconSize.Default );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Handles the icon onclick event.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected Task OnClickHandler()
        {
            return Clicked.InvokeAsync();
        }

        /// <summary>
        /// Handles the icon onmouseover event.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected Task OnMouseOverHandler( MouseEventArgs eventArgs )
        {
            return MouseOver.InvokeAsync( eventArgs );
        }

        /// <summary>
        /// Handles the icon onmouseout event.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected Task OnMouseOutHandler( MouseEventArgs eventArgs )
        {
            return MouseOut.InvokeAsync( eventArgs );
        }

        #endregion

        #region Properties

        /// <summary>
        /// An icon provider that is responsible to give the icon a class-name.
        /// </summary>
        [Inject] protected IIconProvider IconProvider { get; set; }

        /// <summary>
        /// Icon name that can be either a string or <see cref="IconName"/>.
        /// </summary>
        [Parameter]
        public object Name
        {
            get => name;
            set
            {
                name = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Suggested icon style.
        /// </summary>
        [Parameter]
        public IconStyle IconStyle
        {
            get => iconStyle;
            set
            {
                iconStyle = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Defines the icon size.
        /// </summary>
        [Parameter]
        public IconSize IconSize
        {
            get => iconSize;
            set
            {
                iconSize = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs when the icon is clicked.
        /// </summary>
        [Parameter] public EventCallback Clicked { get; set; }

        /// <summary>
        /// Occurs when the mouse has entered the icon area.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> MouseOver { get; set; }

        /// <summary>
        /// Occurs when the mouse has left the icon area.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> MouseOut { get; set; }

        #endregion
    }
}
