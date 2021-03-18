#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    public partial class RatingItem : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

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
            if ( Disabled )
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
            if ( Disabled )
                return;

            IsActive = true;

            await ItemHovered.InvokeAsync( Value );
        }

        protected virtual async Task HandleMouseOut( MouseEventArgs e )
        {
            if ( Disabled )
                return;

            IsActive = false;

            await ItemHovered.InvokeAsync( null );
        }

        #endregion

        #region Properties

        protected object IconName => Rating.IsSelectedRange( Value )
            ? Rating.FullIcon
            : Rating.EmptyIcon;

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

        protected bool IsActive { get; set; }

        protected bool IsSelected => Rating.SelectedValue == Value;

        [CascadingParameter] private Rating Rating { get; set; }

        [Parameter] public int Value { get; set; }

        [Parameter] public Color Color { get; set; } = Color.Warning;

        [Parameter] public bool Disabled { get; set; }

        [Parameter] public bool ReadOnly { get; set; }

        [Parameter] public EventCallback<int> ItemClicked { get; set; }

        [Parameter] public EventCallback<int?> ItemHovered { get; set; }

        #endregion
    }
}
