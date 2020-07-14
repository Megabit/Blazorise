using Blazorise;
using Microsoft.AspNetCore.Components;

namespace ThemeApp
{
    public partial class ThemeCustomizer : ComponentBase
    {
        private string SelectedTab { get; set; } = "General";
        [CascadingParameter] public Theme Theme { get; set; }

        private void OnSelectedTabChanged(string name)
        {
            SelectedTab = name;
        }
    }
}