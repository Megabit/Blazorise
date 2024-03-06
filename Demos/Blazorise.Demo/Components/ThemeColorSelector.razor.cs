using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Demo.Components;
public partial class ThemeColorSelector
{
    [Parameter]
    public string Value
    {
        get => _value;
        set
        {
            if ( value == _value )
            {
                return;
            }
            _value = value;

            InvokeAsync( StateHasChanged );

            ValueChanged.InvokeAsync( value );
        }
    }

    string ClassNames( string value )
        => $"demo-theme-color-item{( value == Value ? " selected" : "" )}";

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    private string _value;

    Task OnClick( string value )
    {
        Value = value;

        return Task.CompletedTask;
    }
}
