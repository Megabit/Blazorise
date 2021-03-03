#region Using directives
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
            Checked = @checked;
            return CheckedChanged.InvokeAsync( @checked );
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

        [CascadingParameter] protected DataGrid<TItem> ParentDataGrid { get; set; }

        [Parameter] public string Width { get; set; }

        [Parameter] public string Class { get; set; }

        [Parameter] public string Style { get; set; }

        [Parameter] public TextAlignment TextAlignment { get; set; }

        [Parameter] public bool Checked { get; set; }

        [Parameter] public EventCallback<bool> CheckedChanged { get; set; }

        #endregion
    }
}