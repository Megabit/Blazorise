﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class _BaseDataGridRow<TItem> : BaseDataGridComponent
    {
        #region Members

        /// <summary>
        /// List of columns used to build this row.
        /// </summary>
        protected IEnumerable<DataGridColumn<TItem>> Columns { get; set; }

        /// <summary>
        /// Holds the internal value for every cell in the row.
        /// </summary>
        protected Dictionary<string, CellEditContext<TItem>> cellValues = new Dictionary<string, CellEditContext<TItem>>();

        /// <summary>
        /// Holds the reference to the multiSelect cell.
        /// </summary>
        protected _DataGridRowMultiSelect<TItem> multiSelect;

        /// <summary>
        /// If click came propagated from MultiSelect Check
        /// Funnels the selection logic into HandleClick.
        /// </summary>
        protected bool clickFromCheck;

        #endregion

        #region Methods

        public override Task SetParametersAsync( ParameterView parameters )
        {
            foreach ( var parameter in parameters )
            {
                switch ( parameter.Name )
                {
                    case nameof( Item ):
                        Item = (TItem)parameter.Value;
                        break;
                    case nameof( ChildContent ):
                        ChildContent = (RenderFragment)parameter.Value;
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

        protected override Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender )
            {
                // initialise all internal cell values
                foreach ( var column in Columns )
                {
                    if ( column.ExcludeFromInit )
                        continue;

                    cellValues.Add( column.ElementId, new CellEditContext<TItem>( Item, ParentDataGrid.UpdateCellEditValue, ParentDataGrid.ReadCellEditValue )
                    {
                        CellValue = column.GetValue( Item ),
                    } );
                }
            }

            return base.OnAfterRenderAsync( firstRender );
        }

        protected internal async Task HandleClick( BLMouseEventArgs eventArgs )
        {
            if ( !clickFromCheck )
                await ParentDataGrid.OnRowClickedCommand( new( Item, eventArgs ) );

            var selectable = ParentDataGrid.RowSelectable?.Invoke( Item ) ?? true;

            if ( !selectable )
                return;

            if ( !clickFromCheck )
                await HandleSingleSelectClick( eventArgs );

            await HandleMultiSelectClick( eventArgs );
            clickFromCheck = false;
        }

        private async Task HandleMultiSelectClick( BLMouseEventArgs eventArgs )
        {
            if ( ParentDataGrid.MultiSelect )
            {
                var isSelected = ( ParentDataGrid.SelectedRows == null || ( ParentDataGrid.SelectedRows != null && !ParentDataGrid.SelectedRows.Any( x => x.IsEqual( Item ) ) ) );
                var shiftClick = ( eventArgs.ShiftKey && eventArgs.Button == MouseButton.Left );

                await OnMultiSelectCommand( isSelected || shiftClick, shiftClick );
            }
        }

        private async Task HandleSingleSelectClick( BLMouseEventArgs eventArgs )
        {
            // Un-select row if the user is holding the ctrl key on already selected row.
            if ( ParentDataGrid.SingleSelect && eventArgs.CtrlKey && eventArgs.Button == MouseButton.Left
                && ParentDataGrid.SelectedRow != null
                && Item.IsEqual( ParentDataGrid.SelectedRow ) )
            {
                await ParentDataGrid.Select( default );
            }
            else if ( !eventArgs.ShiftKey
                && ParentDataGrid.MultiSelect
                && ParentDataGrid.SelectedRows != null
                && ParentDataGrid.SelectedRows.Any( x => x.IsEqual( Item ) ) )
            {
                // If the user selects an already selected multiselect row, seems like it should be more transparent,
                // to just de-select both normal and multi selection
                // Remove this, if that is not the case!!
                await ParentDataGrid.Select( default );
            }
            else
            {
                await ParentDataGrid.Select( Item );
            }
        }

        protected internal Task HandleDoubleClick( BLMouseEventArgs eventArgs )
        {
            return ParentDataGrid.OnRowDoubleClickedCommand( new( Item, eventArgs ) );
        }

        protected internal Task OnMultiSelectCommand( bool selected, bool shiftClick )
        {
            return ParentDataGrid.OnMultiSelectCommand( new( Item, selected, shiftClick ) );
        }

        protected Task OnMultiSelectCheckClicked()
        {
            clickFromCheck = true;
            return Task.CompletedTask;
        }

        protected Cursor GetHoverCursor()
            => ParentDataGrid.RowHoverCursor == null ? Cursor.Pointer : ParentDataGrid.RowHoverCursor( Item );

        protected override Task OnInitializedAsync()
        {
            this.Columns = ParentDataGrid.DisplayableColumns;
            return base.OnInitializedAsync();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates if the row is selected.
        /// </summary>
        protected bool IsSelected =>
            ( ( ParentDataGrid.EditState == DataGridEditState.None || ParentDataGrid.SelectionMode == DataGridSelectionMode.Single ) && ParentDataGrid.SelectedRow.IsEqual( Item ) )
            ||
            ( ParentDataGrid.SelectionMode == DataGridSelectionMode.Multiple && ParentDataGrid.SelectedRows != null && ParentDataGrid.SelectedRows.Any( x => x.IsEqual( Item ) ) );

        /// <summary>
        /// Gets the row background color.
        /// </summary>
        protected Background GetBackground( DataGridRowStyling styling, DataGridRowStyling selectedStyling ) => ( IsSelected
            ? selectedStyling?.Background
            : styling?.Background ) ?? Blazorise.Background.None;

        /// <summary>
        /// Gets the row color.
        /// </summary>
        protected Color GetColor( DataGridRowStyling styling, DataGridRowStyling selectedStyling ) => ( IsSelected
            ? selectedStyling?.Color
            : styling?.Color ) ?? Blazorise.Color.Primary;

        /// <summary>
        /// Gets the row classnames.
        /// </summary>
        protected string GetClass( DataGridRowStyling styling, DataGridRowStyling selectedStyling ) => IsSelected
           ? selectedStyling?.Class
           : styling?.Class;

        /// <summary>
        /// Gets the row styles.
        /// </summary>
        protected string GetStyle( DataGridRowStyling styling, DataGridRowStyling selectedStyling ) => IsSelected
           ? selectedStyling?.Style
           : styling?.Style;

        /// <summary>
        /// Gets or sets the parent <see cref="DataGrid{TItem}"/> of the this component.
        /// </summary>
        [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

        /// <summary>
        /// Item associated with the data set.
        /// </summary>
        [Parameter] public TItem Item { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}