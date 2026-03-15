#region Using directives
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class SelectItem<TValue> : Blazorise.SelectItem<TValue>
{
    #region Methods

    protected override Task OnInitializedAsync()
    {
        ParentSelectBridge?.AddSelectItem( this );

        return base.OnInitializedAsync();
    }

    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentSelectBridge?.RemoveSelectItem( this );
        }

        base.Dispose( disposing );
    }

    protected Task OnClickHandler()
    {
        if ( ParentAntSelect is not null )
        {
            return ParentAntSelect.NotifySelectValueChanged( Value );
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

            if ( IsSelected )
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

    bool IsSelected => ParentSelectBridge?.ContainsValue( Value ) == true;

    [CascadingParameter] ISelect ParentSelectBridge { get; set; }

    IAntSelect ParentAntSelect => ParentSelectBridge as IAntSelect;

    #endregion
}
