#region Using directives
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
                await Clicked.InvokeAsync( new( Item, eventArgs ) );

            var selectable = ParentDataGrid.RowSelectable?.Invoke( Item ) ?? true;

            if ( !selectable )
                return;

            if ( !clickFromCheck )
                await HandleSingleSelectClick( eventArgs );

            await HandleMultiSelectClick( eventArgs );
            clickFromCheck = false;

            await ParentDataGrid.ToggleDetailRow( Item );
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
                await Selected.InvokeAsync( default );
            }
            else if ( !eventArgs.ShiftKey
                && ParentDataGrid.MultiSelect
                && ParentDataGrid.SelectedRows != null
                && ParentDataGrid.SelectedRows.Any( x => x.IsEqual( Item ) ) )
            {
                // If the user selects an already selected multiselect row, seems like it should be more transparent,
                // to just de-select both normal and multi selection
                // Remove this, if that is not the case!!
                await Selected.InvokeAsync( default );
            }
            else
            {
                await Selected.InvokeAsync( Item );
            }
        }

        protected internal Task HandleDoubleClick( BLMouseEventArgs eventArgs )
        {
            return DoubleClicked.InvokeAsync( new( Item, eventArgs ) );
        }

        protected internal Task OnEditCommand()
        {
            return Edit.InvokeAsync( Item );
        }

        protected internal Task OnDeleteCommand()
        {
            return Delete.InvokeAsync( Item );
        }

        protected internal Task OnSaveCommand()
        {
            return Save.InvokeAsync( Item );
        }

        protected internal Task OnCancelCommand()
        {
            return Cancel.InvokeAsync( Item );
        }

        protected internal Task OnMultiSelectCommand( bool selected )
        {
            return MultiSelect.InvokeAsync( new( Item, selected, false ) );
        }

        protected internal Task OnMultiSelectCommand( bool selected, bool shiftClick )
        {
            return MultiSelect.InvokeAsync( new( Item, selected, shiftClick ) );
        }

        protected Task OnMultiSelectCheckClicked()
        {
            clickFromCheck = true;

            return Task.CompletedTask;
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
        /// Item associated with the data set.
        /// </summary>
        [Parameter] public TItem Item { get; set; }

        /// <summary>
        /// List of columns used to build this row.
        /// </summary>
        [Parameter] public IEnumerable<DataGridColumn<TItem>> Columns { get; set; }

        /// <summary>
        /// Gets or sets the parent <see cref="DataGrid{TItem}"/> of the this component.
        /// </summary>
        [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

        /// <summary>
        /// Occurs after the row is selected.
        /// </summary>
        [Parameter] public EventCallback<TItem> Selected { get; set; }

        /// <summary>
        /// Occurs after the row is clicked.
        /// </summary>
        [Parameter] public EventCallback<DataGridRowMouseEventArgs<TItem>> Clicked { get; set; }

        /// <summary>
        /// Occurs after the row is double clicked.
        /// </summary>
        [Parameter] public EventCallback<DataGridRowMouseEventArgs<TItem>> DoubleClicked { get; set; }

        /// <summary>
        /// Activates the edit command for current item.
        /// </summary>
        [Parameter] public EventCallback<TItem> Edit { get; set; }

        /// <summary>
        /// Activates the delete command for current item.
        /// </summary>
        [Parameter] public EventCallback<TItem> Delete { get; set; }

        /// <summary>
        /// Activates the save command.
        /// </summary>
        [Parameter] public EventCallback Save { get; set; }

        /// <summary>
        /// Activates the cancel command.
        /// </summary>
        [Parameter] public EventCallback Cancel { get; set; }

        /// <summary>
        /// Activates the multi select command.
        /// </summary>
        [Parameter] public EventCallback<MultiSelectEventArgs<TItem>> MultiSelect { get; set; }

        /// <summary>
        /// Gets or sets the applied cursor when the row is hovered over.
        /// </summary>
        [Parameter] public Cursor HoverCursor { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Holds the basic information about the datagrid row.
        /// </summary>
        [Parameter] public DataGridRowInfo<TItem> RowInfo { get; set; }

        /// <summary>
        /// A trigger function used to handle the visibility of detail row.
        /// </summary>
        [Parameter] public Func<TItem, bool> DetailRowTrigger { get; set; }

        #endregion
    }
}