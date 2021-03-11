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
            FullIcon = IconProvider.GetIconName( IconName.Star );
            EmptyIcon = IconProvider.GetIconName( IconName.Star );
        }

        /// <summary>
        /// User class names for RatingItems, separated by space
        /// </summary>
        [Parameter] public string RatingItemsClass { get; set; }

        /// <summary>
        /// User styles for RatingItems.
        /// </summary>
        [Parameter] public string RatingItemsStyle { get; set; }

        /// <summary>
        /// Input name. If not initialized, name will be random guid.
        /// </summary>
        [Parameter] public string Name { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Max value and how many elements to click will be generated. Default: 5
        /// </summary>
        [Parameter] public int MaxValue { get; set; } = 5;

        /// <summary>
        /// Selected or hovered icon. Default @Icons.Material.Star
        /// </summary>
        [Parameter] public string FullIcon { get; set; }

        /// <summary>
        /// Non selected item icon. Default @Icons.Material.StarBorder
        /// </summary>
        [Parameter] public string EmptyIcon { get; set; }

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Primary;
        /// <summary>
        /// The Size of the icons.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;
        /// <summary>
        /// If true, the controls will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }
        /// <summary>
        /// If true, the ratings will show without interactions.
        /// </summary>
        [Parameter] public bool ReadOnly { get; set; }

        /// <summary>
        /// Fires when SelectedValue changes.
        /// </summary>
        [Parameter] public EventCallback<int> SelectedValueChanged { get; set; }

        /// <summary>
        /// Selected value. This property is two-way bindable.
        /// </summary>
        [Parameter]
        public int SelectedValue
        {
            get => _selectedValue;
            set
            {
                if ( _selectedValue == value )
                    return;

                _selectedValue = value;

                SelectedValueChanged.InvokeAsync( _selectedValue );
            }
        }

        private int _selectedValue = 0;

        /// <summary>
        /// Fires when hovered value change. Value will be null if no rating item is hovered.
        /// </summary>
        [Parameter] public EventCallback<int?> HoveredValueChanged { get; set; }

        internal int? HoveredValue
        {
            get => _hoveredValue;
            set
            {
                if ( _hoveredValue == value )
                    return;

                _hoveredValue = value;
                HoveredValueChanged.InvokeAsync( value );
            }
        }

        private int? _hoveredValue = null;

        internal bool IsRatingHover => HoveredValue.HasValue;

        private void HandleItemClicked( int itemValue )
        {
            SelectedValue = itemValue;

            if ( itemValue == 0 )
            {
                HoveredValue = null;
            }
        }

        private void HandleItemHovered( int? itemValue ) => HoveredValue = itemValue;
    }
}
