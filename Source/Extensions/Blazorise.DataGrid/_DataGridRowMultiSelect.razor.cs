﻿#region Using directives
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridRowMultiSelect<TItem> : ComponentBase
    {
        #region Methods

        internal Task OnCheckedChanged( bool @checked )
        {
            //Multi Select Checked State is bound to the Row Selected State
            return CheckedChanged.InvokeAsync( Checked );
        }

        internal Task OnCheckedClicked()
        {
            return CheckedClicked.InvokeAsync();
        }

        protected string BuildCellStyle()
        {
            var sb = new StringBuilder();

            if ( !string.IsNullOrEmpty( Style ) )
                sb.Append( Style );

            if ( Width != null )
                sb.Append( $"; width: {Width}" );

            return sb.ToString().TrimStart( ' ', ';' );
        }

        #endregion

        #region Properties

        [Parameter] public TItem Item { get; set; }

        /// <summary>
        /// Gets or sets the parent <see cref="DataGrid{TItem}"/> of the this component.
        /// </summary>
        [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

        [Parameter] public string Width { get; set; }

        [Parameter] public string Class { get; set; }

        [Parameter] public string Style { get; set; }

        [Parameter] public TextAlignment TextAlignment { get; set; }

        [Parameter] public VerticalAlignment VerticalAlignment { get; set; }

        [Parameter] public bool Checked { get; set; }

        [Parameter] public EventCallback<bool> CheckedChanged { get; set; }

        [Parameter] public EventCallback CheckedClicked { get; set; }

        #endregion
    }
}