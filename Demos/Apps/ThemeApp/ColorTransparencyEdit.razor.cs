using Microsoft.AspNetCore.Components;

namespace ThemeApp
{
    public partial class ColorTransparencyEdit : ComponentBase
    {
        private string _color;
        [Parameter] public string Color { get => _color; set { if (_color == value) return; _color = value; ColorChanged.InvokeAsync(value); } }
        [Parameter] public EventCallback<string> ColorChanged { get; set; }
        [Parameter] public string Label { get; set; }
    }
}