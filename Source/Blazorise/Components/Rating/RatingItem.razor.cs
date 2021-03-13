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

        protected override void BuildStyles( StyleBuilder builder )
        {
            var selected = Rating.IsSelected( Value );
            var hovered = Rating.IsHovered( Value );

            if ( selected )
            {
                builder.Append( "color: orange;" );
            }
            else if ( hovered )
            {
                builder.Append( "color: orange; opacity: 0.5;" );
            }

            base.BuildStyles( builder );
        }

        private async Task HandleClick()
        {
            if ( Disabled )
                return;

            IsActive = false;

            if ( Rating.SelectedValue == Value )
            {
                await ItemClicked.InvokeAsync( 0 );
            }
            else
            {
                await ItemClicked.InvokeAsync( Value );
            }
        }

        private async Task HandleMouseOver( MouseEventArgs e )
        {
            if ( Disabled )
                return;

            IsActive = true;

            await ItemHovered.InvokeAsync( Value );
        }

        private async Task HandleMouseOut( MouseEventArgs e )
        {
            if ( Disabled )
                return;

            IsActive = false;

            await ItemHovered.InvokeAsync( null );
        }

        #endregion

        #region Properties

        protected object IconName => Rating.IsSelected( Value )
            ? Rating.FullIcon
            : Rating.EmptyIcon;

        protected IconStyle IconStyle => ( Rating.IsSelected( Value ) || Rating.IsHovered( Value )
            ? Rating.FullIconStyle
            : Rating.EmptyIconStyle ) ?? IconStyle.Solid;

        protected bool IsActive { get; set; }

        [CascadingParameter] private Rating Rating { get; set; }

        [Parameter] public int Value { get; set; }

        /// <summary>
        /// Not work
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Primary;

        [Parameter] public bool Disabled { get; set; }

        [Parameter] public bool ReadOnly { get; set; }

        [Parameter] public EventCallback<int> ItemClicked { get; set; }

        [Parameter] public EventCallback<int?> ItemHovered { get; set; }

        #endregion
    }
}
