using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Blazorise
{
    public partial class RatingItem : BaseComponent
    {
        [CascadingParameter]
        private Rating Rating { get; set; }

        [Inject] public IClassProvider classProvider { get; set; }

        [Parameter] public int ItemValue { get; set; }

        internal string Name { get; set; }

        internal bool IsActive { get; set; }

        private bool IsChecked => ItemValue == Rating?.SelectedValue;

        /// <summary>
        /// The Size of the icon.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// If true, the controls will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// If true, the item will be readonly.
        /// </summary>
        [Parameter] public bool ReadOnly { get; set; }

        [Parameter] public IconStyle IconStyle { get; set; }

        /// <summary>
        /// Fires when element clicked.
        /// </summary>
        [Parameter] public EventCallback<int> ItemClicked { get; set; }

        /// <summary>
        /// Fires when element hovered.
        /// </summary>
        [Parameter] public EventCallback<int?> ItemHovered { get; set; }

        protected override void BuildClasses( ClassBuilder builder )
        {
            // I need to color icon, but not work fine
            builder.Append( $"color: {classProvider.ToColor( Color )}" );
            base.BuildClasses( builder );
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            var select = SelectIcon();
            Name = select.Name;
            IconStyle = select.IconStyle;
        }

        private (string Name, IconStyle IconStyle) SelectIcon()
        {
            if ( Rating == null )
                return (null, IconStyle.Regular);
            if ( Rating.HoveredValue.HasValue && Rating.HoveredValue.Value >= ItemValue )
            {
                // full icon when @RatingItem hovered
                return (Rating.FullIcon, IconStyle.Solid);
            }
            else if ( Rating.SelectedValue >= ItemValue )
            {
                if ( Rating.HoveredValue.HasValue && Rating.HoveredValue.Value < ItemValue )
                {
                    // empty icon when equal or higher RatingItem value clicked, but less value hovered 
                    return (Rating.EmptyIcon, IconStyle.Regular);
                }
                else
                {
                    // full icon when equal or higher RatingItem value clicked
                    return (Rating.FullIcon, IconStyle.Solid);
                }
            }
            else
            {
                // empty icon when this or higher RatingItem is not clicked and not hovered
                return (Rating.EmptyIcon, IconStyle.Regular);
            }
        }

        // rating item lose hover
        private async Task HandleMouseOut( MouseEventArgs e )
        {
            if ( Disabled )
                return;
            if ( Rating == null )
                return;

            IsActive = false;
            await ItemHovered.InvokeAsync( null );
        }

        private void HandleMouseOver( MouseEventArgs e )
        {
            if ( Disabled )
                return;

            IsActive = true;
            ItemHovered.InvokeAsync( ItemValue );
        }

        private void HandleClick( MouseEventArgs e )
        {
            if ( Disabled )
                return;
            IsActive = false;
            if ( Rating?.SelectedValue == ItemValue )
            {
                ItemClicked.InvokeAsync( 0 );
            }
            else
            {
                ItemClicked.InvokeAsync( ItemValue );
            }
        }
    }
}
