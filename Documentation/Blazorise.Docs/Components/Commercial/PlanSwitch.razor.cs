using System;
using System.Threading.Tasks;
using Blazorise.Docs.Services;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Docs.Components.Commercial;

public partial class PlanSwitch : IDisposable
{
    private Background GetSelectedBackground( string value ) => SelectedValue == value ? Background.Primary : Background.Default;

    private TextColor GetTextSelectedColor( string value ) => SelectedValue == value
        ? TextColor.White
        : ThemeService.IsDark ? TextColor.Dark : TextColor.Default;

    private Task OnClicked( string value )
    {
        return SelectedValueChanged.InvokeAsync( value );
    }

    protected override void OnInitialized()
    {
        ThemeService.ThemeChanged += OnThemeChanged;

        base.OnInitialized();
    }

    public void Dispose()
    {
        ThemeService.ThemeChanged -= OnThemeChanged;
    }

    void OnThemeChanged( object sender, string theme )
    {
        InvokeAsync( StateHasChanged );
    }

    [Inject] private ThemeService ThemeService { get; set; }

    [Parameter] public string SelectedValue { get; set; }

    [Parameter] public EventCallback<string> SelectedValueChanged { get; set; }
}
