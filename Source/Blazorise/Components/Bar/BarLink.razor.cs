#region Using directives
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A clickable link, the sibling of a <see cref="BarItem"/> or <see cref="BarDropdown"/>.
    /// </summary>
    public partial class BarLink : BaseComponent
    {
        #region Members

        private BarItemState parentItemState;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarLink( ParentBarItemState?.Mode ?? BarMode.Horizontal ) );
            builder.Append( ClassProvider.BarLinkDisabled( ParentBarItemState?.Mode ?? BarMode.Horizontal ), ParentBarItemState?.Disabled ?? false );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Handles the link onclick event.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected Task ClickHandler()
        {
            return Clicked.InvokeAsync( null );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] public EventCallback Clicked { get; set; }

        /// <summary>
        /// Specifies the URL of the page the link goes to.
        /// </summary>
        [Parameter] public string To { get; set; }

        /// <summary>
        /// The target attribute specifies where to open the linked document.
        /// </summary>
        [Parameter] public Target Target { get; set; } = Target.None;

        /// <summary>
        /// URL matching behavior for a link.
        /// </summary>
        [Parameter] public Match Match { get; set; } = Match.All;

        /// <summary>
        /// Specify extra information about the element.
        /// </summary>
        [Parameter] public string Title { get; set; }

        /// <summary>
        /// Cascaded <see cref="Bar"/> component state object.
        /// </summary>
        [CascadingParameter]
        protected BarItemState ParentBarItemState
        {
            get => parentItemState;
            set
            {
                if ( parentItemState == value )
                    return;

                parentItemState = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="BarLink"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
