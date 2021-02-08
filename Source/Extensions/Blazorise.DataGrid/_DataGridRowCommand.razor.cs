#region Using directives
using System;
using System.Text;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridRowCommand<TItem> : ComponentBase, IDisposable
    {
        protected override void OnInitialized()
        {
            LocalizerService.LocalizationChanged += OnLocalizationChanged;

            base.OnInitialized();
        }

        public void Dispose()
        {
            LocalizerService.LocalizationChanged -= OnLocalizationChanged;
        }

        private async void OnLocalizationChanged( object sender, EventArgs e )
        {
            await InvokeAsync( StateHasChanged );
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

        [Inject] protected ITextLocalizerService LocalizerService { get; set; }

        [Inject] protected ITextLocalizer<DataGrid<TItem>> Localizer { get; set; }

        [Parameter] public TItem Item { get; set; }

        [Parameter] public DataGridEditState EditState { get; set; }

        [Parameter] public EventCallback Edit { get; set; }

        [Parameter] public EventCallback Delete { get; set; }

        [Parameter] public EventCallback Save { get; set; }

        [Parameter] public EventCallback Cancel { get; set; }

        [CascadingParameter] protected DataGrid<TItem> ParentDataGrid { get; set; }

        [Parameter] public string Width { get; set; }

        [Parameter] public string Class { get; set; }

        [Parameter] public string Style { get; set; }

        [Parameter] public TextAlignment TextAlignment { get; set; }
    }
}
