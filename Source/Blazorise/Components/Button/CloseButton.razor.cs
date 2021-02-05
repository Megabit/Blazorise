#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A generic close button for dismissing content like modals and alerts.
    /// </summary>
    public partial class CloseButton : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CloseButton() );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Handles the item onclick event.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected Task ClickHandler()
        {
            ParentAlert?.Hide();
            ParentModal?.Hide();
            return Clicked.InvokeAsync( null );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Occurs when the button is clicked.
        /// </summary>
        [Parameter] public EventCallback Clicked { get; set; }

        /// <summary>
        /// Cascaded <see cref="Alert"/> component in which this <see cref="CloseButton"/> is placed.
        /// </summary>
        [CascadingParameter] protected Alert ParentAlert { get; set; }

        /// <summary>
        /// Cascaded <see cref="Modal/> component in which this <see cref="CloseButton"/> is placed.
        /// </summary>
        [CascadingParameter] protected Modal ParentModal { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="CloseButton"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
