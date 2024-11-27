#region Using directives
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class SelectItem<TValue> : Blazorise.SelectItem<TValue>
{
    #region Methods

    protected Task OnClickHandler()
    {
        if ( ParentSelect != null && ParentSelect is IAntSelect select )
        {
            return select.NotifySelectValueChanged( Value );
        }

        return Task.CompletedTask;
    }

    protected Task OnMouseOverHandler( MouseEventArgs eventArgs )
    {
        Active = true;

        return Task.CompletedTask;
    }

    protected Task OnMouseOutHandler( MouseEventArgs eventArgs )
    {
        Active = false;

        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    string SelectItemClassNames
    {
        get
        {
            var sb = new StringBuilder( $"{ClassNames} ant-select-item ant-select-item-option" );

            if ( Selected )
            {
                sb.Append( " ant-select-item-option-selected" );
            }

            if ( Active )
            {
                sb.Append( " ant-select-item-option-active" );
            }

            if ( Disabled )
            {
                sb.Append( " ant-select-item-option-disabled" );
            }

            return sb.ToString();
        }
    }

    bool Active { get; set; }

    string ActiveString => Active ? "true" : "false";

    #endregion
}