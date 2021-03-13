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

        //internal string Name { get; set; }

        //internal bool IsActive { get; set; }

        //private bool IsChecked => ItemValue == Rating?.SelectedValue;

        #endregion

        #region Methods

        //protected override void OnParametersSet()
        //{
        //    base.OnParametersSet();
        //    var select = SelectIcon();
        //    Name = select.Name;
        //    IconStyle = select.IconStyle;
        //}

        protected override void BuildStyles( StyleBuilder builder )
        {
            if ( Rating.IsChecked( Value ) )
            {
                builder.Append( "color: orange;" );
            }

            base.BuildStyles( builder );
        }

        private async Task HandleClick()
        {
            if ( Disabled )
                return;

            IsActive = false;

            if ( Rating?.SelectedValue == Value )
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
            if ( Rating == null )
                return;

            IsActive = false;

            await ItemHovered.InvokeAsync( null );
        }

        //private (string Name, IconStyle IconStyle) SelectIcon()
        //{
        //    if ( Rating == null )
        //        return (null, IconStyle.Solid);
        //    if ( Rating.HoveredValue.HasValue && Rating.HoveredValue.Value >= ItemValue )
        //    {
        //        // full icon when @RatingItem hovered
        //        return (Rating.FullIconName, Rating.FullIconStyle.Value);
        //    }
        //    else if ( Rating.SelectedValue >= ItemValue )
        //    {
        //        if ( Rating.HoveredValue.HasValue && Rating.HoveredValue.Value < ItemValue )
        //        {
        //            // empty icon when equal or higher RatingItem value clicked, but less value hovered 
        //            return (Rating.EmptyIconName, Rating.EmptyIconStyle.Value);
        //        }
        //        else
        //        {
        //            // full icon when equal or higher RatingItem value clicked
        //            return (Rating.FullIconName, Rating.FullIconStyle.Value);
        //        }
        //    }
        //    else
        //    {
        //        // empty icon when this or higher RatingItem is not clicked and not hovered
        //        return (Rating.EmptyIconName, Rating.EmptyIconStyle.Value);
        //    }
        //}

        #endregion

        #region Properties

        protected object IconName => Rating.IsChecked( Value ) || IsActive
            ? Rating.FullIcon
            : Rating.EmptyIcon;

        protected IconStyle IconStyle => ( Rating.IsChecked( Value ) || IsActive
            ? Rating.FullIconStyle
            : Rating.EmptyIconStyle ) ?? Blazorise.IconStyle.Solid;

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
