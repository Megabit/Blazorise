#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines a cell as header of a group of table cells.
    /// </summary>
    public partial class TableHeaderCell : BaseDraggableComponent
    {
        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TableHeaderCell() );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Handles the header cell clicked event.
        /// </summary>
        /// <param name="eventArgs">Supplies information about a mouse event that is being raised.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected Task OnClickHandler( MouseEventArgs eventArgs )
        {
            return Clicked.InvokeAsync( EventArgsMapper.ToMouseEventArgs( eventArgs ) );
        }

        /// <inheritdoc/>
        protected override Task OnParametersSetAsync()
        {
            SetFixedHeaderBackground();
            return base.OnParametersSetAsync();
        }

        private void SetFixedHeaderBackground()
        {
            var bgWhiteClass = "bg-white";
            if ( ParentTable.FixedHeader )
            {
                if ( ParentTable.Background == Background.None && ( !this.Class?.Contains( bgWhiteClass ) ?? true ))
                { 
                    Class += $" {bgWhiteClass}";
                }
                if ( ParentTable.Background != Background.None && this.Background == Background.None )
                    this.Background = ParentTable.Background;
            }
            else
            {
                var ocorrence = this.Class?.IndexOf( bgWhiteClass ) ?? -1;
                if ( ocorrence > 0 )
                    _ = Class.Remove( ocorrence, bgWhiteClass.Length );
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Cascading Parent Table
        /// </summary>
        [CascadingParameter] public Table ParentTable { get; set; }
        /// <summary>
        /// Number of rows a cell should span.
        /// </summary>
        [Parameter] public int? RowSpan { get; set; }

        /// <summary>
        /// Number of columns a cell should span.
        /// </summary>
        [Parameter] public int? ColumnSpan { get; set; }

        /// <summary>
        /// Occurs when the header cell is clicked.
        /// </summary>
        [Parameter] public EventCallback<BLMouseEventArgs> Clicked { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="TableHeaderCell"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
