using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Blazorise
{
    public partial class RatingItem : BaseComponent
    {
        [CascadingParameter]
        private AwRating Rating { get; set; }

        [Inject] public IClassProvider classProvider { get; set; }

        [Parameter] public int ItemValue { get; set; }

        internal string Name { get; set; }

        internal bool IsActive { get; set; }

        private bool IsChecked => ItemValue == Rating?.SelectedValue;

        /// <summary>
        /// Not work now
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// Not work
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Primary;

        [Parameter] public bool Disabled { get; set; }

        [Parameter] public bool ReadOnly { get; set; }

        [Parameter] public IconStyle IconStyle { get; set; }

        [Parameter] public EventCallback<int> ItemClicked { get; set; }

        [Parameter] public EventCallback<int?> ItemHovered { get; set; }

        protected override void BuildClasses(ClassBuilder builder)
        {
            builder.Append($"color: {classProvider.ToColor(Color)}");
            base.BuildClasses(builder);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            var select = SelectIcon();
            Name = select.Name;
            IconStyle = select.IconStyle;
        }

        private (string Name, IconStyle  IconStyle) SelectIcon()
        {
            if (Rating == null)
                return (null, IconStyle.Solid);
            if (Rating.HoveredValue.HasValue && Rating.HoveredValue.Value >= ItemValue)
            {
                // full icon when @RatingItem hovered
                return (Rating.FullIconName, Rating.FullIconStyle.Value);
            }
            else if (Rating.SelectedValue >= ItemValue)
            {
                if (Rating.HoveredValue.HasValue && Rating.HoveredValue.Value < ItemValue)
                {
                    // empty icon when equal or higher RatingItem value clicked, but less value hovered 
                    return (Rating.EmptyIconName, Rating.EmptyIconStyle.Value);
                }
                else
                {
                    // full icon when equal or higher RatingItem value clicked
                    return (Rating.FullIconName, Rating.FullIconStyle.Value);
                }
            }
            else
            {
                // empty icon when this or higher RatingItem is not clicked and not hovered
                return (Rating.EmptyIconName, Rating.EmptyIconStyle.Value);
            }
        }

        // rating item lose hover
        private async Task HandleMouseOut(MouseEventArgs e)
        {
            if (Disabled) return;
            if (Rating == null)
                return;

            IsActive = false;
            await ItemHovered.InvokeAsync(null);
        }

        private void HandleMouseOver(MouseEventArgs e)
        {
            if (Disabled) return;

            IsActive = true;
            ItemHovered.InvokeAsync(ItemValue);
        }

        private void HandleClick(MouseEventArgs e)
        {
            if (Disabled) return;
            IsActive = false;
            if (Rating?.SelectedValue == ItemValue)
            {
                ItemClicked.InvokeAsync(0);
            }
            else
            {
                ItemClicked.InvokeAsync(ItemValue);
            }
        }
    }
}
