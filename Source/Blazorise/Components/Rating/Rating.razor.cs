#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    public partial class Rating : BaseComponent
    {
        #region Members

        private int selectedValue = 0;

        private int? hoveredValue = null;

        private bool hovering;

        #endregion

        #region Methods

        private Task HandleItemClicked( int value )
        {
            SelectedValue = value;

            if ( value == 0 )
            {
                HoveredValue = null;
            }

            return Task.CompletedTask;
        }

        private Task HandleItemHovered( int? value )
        {
            HoveredValue = value;

            return Task.CompletedTask;
        }

        protected Task OnMouseOverHandler( MouseEventArgs eventArgs )
        {
            hovering = true;

            return Task.CompletedTask;
        }

        protected Task OnMouseOutHandler( MouseEventArgs eventArgs )
        {
            hovering = false;

            return Task.CompletedTask;
        }

        internal protected bool IsSelected( int value )
            => value >= 1 && value <= SelectedValue;

        internal protected bool IsHovered( int value )
           => hovering && value >= 1 && value <= HoveredValue;

        #endregion

        #region Properties

        [Inject] IIconProvider IconProvider { get; set; }

        /// <summary>
        /// User class names for RatingItems, separated by space
        /// </summary>
        [Parameter] public string RatingItemsClass { get; set; }

        /// <summary>
        /// User styles for RatingItems.
        /// </summary>
        [Parameter] public string RatingItemsStyle { get; set; }

        [Parameter] public int MaxValue { get; set; } = 5;

        [Parameter] public object FullIcon { get; set; } = IconName.Star;

        [Parameter] public object EmptyIcon { get; set; } = IconName.Star;

        [Parameter] public IconStyle? FullIconStyle { get; set; } = IconStyle.Solid;

        [Parameter] public IconStyle? EmptyIconStyle { get; set; } = IconStyle.Regular;

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

        [Parameter]
        public int SelectedValue
        {
            get => selectedValue;
            set
            {
                if ( selectedValue == value )
                    return;

                selectedValue = value;

                SelectedValueChanged.InvokeAsync( selectedValue );
            }
        }

        [Parameter] public EventCallback<int> SelectedValueChanged { get; set; }

        internal int? HoveredValue
        {
            get => hoveredValue;
            set
            {
                if ( value == null || hoveredValue == value )
                    return;

                hoveredValue = value;
                HoveredValueChanged.InvokeAsync( value );
            }
        }

        [Parameter] public EventCallback<int?> HoveredValueChanged { get; set; }

        #endregion
    }
}
