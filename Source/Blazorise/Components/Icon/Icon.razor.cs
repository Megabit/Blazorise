#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    /// <summary>
    /// An icon component.
    /// </summary>
    public partial class Icon : BaseComponent
    {
        #region Members

        private object name;

        private IconStyle iconStyle = IconStyle.Solid;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( IconProvider.Icon( Name, IconStyle ) );

            base.BuildClasses( builder );
        }

        protected Task OnClickHandler()
        {
            return Clicked.InvokeAsync( null );
        }

        protected Task OnMouseOverHandler( MouseEventArgs eventArgs )
        {
            return MouseOver.InvokeAsync( eventArgs );
        }

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
        /// Occurs when the icon is clicked.
        /// </summary>
        [Parameter] public EventCallback Clicked { get; set; }

        [Parameter] public EventCallback<MouseEventArgs> MouseOver { get; set; }

        [Parameter] public EventCallback<MouseEventArgs> MouseOut { get; set; }

        #endregion
    }
}
