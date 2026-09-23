using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Docs.Components.Commercial;

public partial class PlanSwitch
{
    private Task OnClicked( string value )
        => SelectedValueChanged.InvokeAsync( value );

    [Parameter] public string SelectedValue { get; set; }

    [Parameter] public EventCallback<string> SelectedValueChanged { get; set; }
}