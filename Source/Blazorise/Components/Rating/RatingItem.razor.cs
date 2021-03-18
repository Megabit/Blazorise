#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Represents the each individual item in the <see cref="Blazorise.Rating"/> component.
    /// </summary>
    public partial class RatingItem : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            var selected = Rating.IsSelectedRange( Value );
            var hovered = Rating.IsHoveredRange( Value );

            builder.Append( ClassProvider.RatingItem() );
            builder.Append( ClassProvider.RatingItemColor( Color ), Color != Color.None );
            builder.Append( ClassProvider.RatingItemSelected( selected ) );
            builder.Append( ClassProvider.RatingItemHovered( hovered ) );

            base.BuildClasses( builder );
        }

        protected virtual async Task HandleClick()
        {
            if ( Rating.Disabled )
                return;

            IsActive = false;

            if ( IsSelected )
            {
                await ItemClicked.InvokeAsync( 0 );
            }
            else
            {
                await ItemClicked.InvokeAsync( Value );
            }
        }

        protected virtual async Task HandleMouseOver( MouseEventArgs e )
        {
            if ( Rating.Disabled )
                return;

            IsActive = true;

            await ItemHovered.InvokeAsync( Value );
        }

        protected virtual async Task HandleMouseOut( MouseEventArgs e )
        {
            if ( Rating.Disabled )
                return;

            IsActive = false;

            await ItemHovered.InvokeAsync( null );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the icon name based on the item state.
        /// </summary>
        protected object IconName => Rating.IsSelectedRange( Value )
            ? Rating.FullIcon
            : Rating.EmptyIcon;

        /// <summary>
        /// Gets the icon style based on the item state.
        /// </summary>
        protected IconStyle IconStyle
        {
            get
            {
                if ( Rating.IsSelectedRange( Value ) )
                    return Rating.FullIconStyle ?? IconStyle.Solid;

                if ( Rating.IsHoveredRange( Value ) )
                    return Rating.EmptyIconStyle ?? IconStyle.Regular;

                return Rating.EmptyIconStyle ?? IconStyle.Regular;
            }
        }

        /// <summary>
        /// Indicates if the item has a focus.
        /// </summary>
        protected bool IsActive { get; set; }

        /// <summary>
        /// Indicate if the item is selected.
        /// </summary>
        protected bool IsSelected => Rating.SelectedValue == Value;

        /// <summary>
        /// Gets or sets the item value.
        /// </summary>
        [Parameter] public int Value { get; set; }

        /// <summary>
        /// Gets or sets the item color.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Warning;

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] public EventCallback<int> ItemClicked { get; set; }

        /// <summary>
        /// Occurs when the item is hovered.
        /// </summary>
        [Parameter] public EventCallback<int?> ItemHovered { get; set; }

        /// <summary>
        /// Cascaded parent rating component.
        /// </summary>
        [CascadingParameter] protected Rating Rating { get; set; }

        #endregion
    }
}
