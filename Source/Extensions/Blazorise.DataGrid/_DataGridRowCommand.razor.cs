﻿#region Using directives
using System;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridRowCommand<TItem> : ComponentBase, IDisposable
    {
        public override Task SetParametersAsync( ParameterView parameters )
        {
            foreach ( var parameter in parameters )
            {
                switch ( parameter.Name )
                {
                    case nameof( Item ):
                        Item = (TItem)parameter.Value;
                        break;
                    case nameof( Column ):
                        Column = (DataGridColumn<TItem>)parameter.Value;
                        break;
                    case nameof( Save ):
                        Save = (EventCallback)parameter.Value;
                        break;
                    case nameof( EditState ):
                        EditState = (DataGridEditState)parameter.Value;
                        break;
                    case nameof( ParentDataGrid ):
                        ParentDataGrid = (DataGrid<TItem>)parameter.Value;
                        break;
                    default:
                        throw new ArgumentException( $"Unknown parameter: {parameter.Name}" );
                }
            }
            return base.SetParametersAsync( ParameterView.Empty );
        }

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
            var style = Column.BuildCellStyle( Item );
            var width = Column.Width;

            var sb = new StringBuilder();

            if ( !string.IsNullOrEmpty( style ) )
                sb.Append( style );

            if ( width != null )
                sb.Append( $"; width: {width}" );

            return sb.ToString().TrimStart( ' ', ';' );
        }

        protected EventCallback Edit
            => EventCallback.Factory.Create( this, () => ParentDataGrid.Edit( Item ) );

        protected EventCallback Delete
            => EventCallback.Factory.Create( this, () => ParentDataGrid.Delete( Item ) );

        protected EventCallback Cancel
            => EventCallback.Factory.Create( this, ParentDataGrid.Cancel );

        [Inject] protected ITextLocalizerService LocalizerService { get; set; }

        [Inject] protected ITextLocalizer<DataGrid<TItem>> Localizer { get; set; }

        [Parameter] public DataGridEditState EditState { get; set; }

        [Parameter] public TItem Item { get; set; }

        [Parameter] public EventCallback Save { get; set; }

        /// <summary>
        /// Gets or sets the parent <see cref="DataGrid{TItem}"/> of the this component.
        /// </summary>
        [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

        [Parameter] public DataGridColumn<TItem> Column { get; set; }
    }
}