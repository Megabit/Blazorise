using System;
using Microsoft.AspNetCore.Components;

namespace Blazorise
{
    public partial class Rating : BaseComponent
    {
        [Inject] IIconProvider IconProvider { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitializedAsync();
            if (FullIconStyle == null)
                FullIconStyle = IconStyle.Solid;
            if (EmptyIconStyle == null)
                EmptyIconStyle = IconStyle.Regular;
            FullIconName = string.IsNullOrEmpty(FullIcon) ? IconProvider.GetIconName(IconName.Star) : FullIcon;
            EmptyIconName = string.IsNullOrEmpty(EmptyIcon) ? IconProvider.GetIconName(IconName.Star) : EmptyIcon;
        }

        /// <summary>
        /// User class names for RatingItems, separated by space
        /// </summary>
        [Parameter] public string RatingItemsClass { get; set; }

        /// <summary>
        /// User styles for RatingItems.
        /// </summary>
        [Parameter] public string RatingItemsStyle { get; set; }

        [Parameter] public string Name { get; set; } = Guid.NewGuid().ToString();

        [Parameter] public int MaxValue { get; set; } = 5;

        [Parameter] public string FullIcon { get; set; }
        public string FullIconName { get; set; }

        [Parameter] public string EmptyIcon { get; set; }
        public string EmptyIconName { get; set; }

        [Parameter] public IconStyle? FullIconStyle { get; set; }
        
        [Parameter] public IconStyle? EmptyIconStyle { get; set; }

        /// <summary>
        /// Not work now
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Primary;
        /// <summary>
        /// Not work now
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public EventCallback<int> SelectedValueChanged { get; set; }

        [Parameter]
        public int SelectedValue
        {
            get => _selectedValue;
            set
            {
                if (_selectedValue == value)
                    return;

                _selectedValue = value;

                SelectedValueChanged.InvokeAsync(_selectedValue);
            }
        }

        private int _selectedValue = 0;

        [Parameter] public EventCallback<int?> HoveredValueChanged { get; set; }

        internal int? HoveredValue
        {
            get => _hoveredValue;
            set
            {
                if (value == null || _hoveredValue == value)
                    return;

                _hoveredValue = value;
                HoveredValueChanged.InvokeAsync(value);
            }
        }

        private int? _hoveredValue = null;

        internal bool IsRatingHover => HoveredValue.HasValue;

        private void HandleItemClicked(int itemValue)
        {
            SelectedValue = itemValue;

            if (itemValue == 0)
            {
                HoveredValue = null;
            }
        }

        private void HandleItemHovered(int? itemValue) => HoveredValue = itemValue;
    }
}
