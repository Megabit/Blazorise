#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        protected Dictionary<string, CellEditContext<TItem>> cellsValues = new Dictionary<string, CellEditContext<TItem>>();

        protected _DataGridRowMultiSelect<TItem> multiSelect;

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

                    cellsValues.Add( column.ElementId, new CellEditContext<TItem>( Item )
                    {
                        CellValue = column.GetValue( Item ),
                    } );
                }
            }

            return base.OnAfterRenderAsync( firstRender );
        }

        protected internal async Task HandleClick( BLMouseEventArgs eventArgs )
        {
            await Clicked.InvokeAsync( new DataGridRowMouseEventArgs<TItem>( Item, eventArgs ) );
            var selectable = ParentDataGrid.RowSelectable?.Invoke( Item ) ?? true;

            if ( !selectable )
                return;

            // un-select row if the user is holding the ctrl key on already selected row
            if ( eventArgs.CtrlKey && eventArgs.Button == MouseButton.Left
                && ParentDataGrid.SelectedRow != null
                && (object)Item == (object)ParentDataGrid.SelectedRow )
            {
                await Selected.InvokeAsync( default );
            }
            else if ( ParentDataGrid.MultiSelect && ParentDataGrid.SelectedRows != null && ParentDataGrid.SelectedRows.Any( x => (object)x == (object)Item ) )
            {
                //If the user selects an already selected multiselect row, seems like it should be more transparent, to just de-select both normal and multi selection
                //Remove this, if that is not the case
                await Selected.InvokeAsync( default );
            }
            else
            {
                await Selected.InvokeAsync( Item );
            }

            if ( ParentDataGrid.MultiSelect )
            {
                if ( multiSelect != null )
                {
                    await multiSelect.IsCheckedChanged( !multiSelect.IsChecked );
                }
                else
                {
                    await OnMultiSelectCommand( ParentDataGrid.SelectedRows != null && !ParentDataGrid.SelectedRows.Any( x => (object)x == (object)Item ) );
                }
            }
        }

        protected internal Task HandleDoubleClick( BLMouseEventArgs eventArgs )
        {
            return DoubleClicked.InvokeAsync( new DataGridRowMouseEventArgs<TItem>( Item, eventArgs ) );
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

        protected internal Task OnMultiSelectCommand( bool IsSelected )
        {
            return MultiSelect.InvokeAsync( (IsSelected, Item) );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates if the row is selected.
        /// </summary>
        protected bool IsSelected => ( ParentDataGrid.EditState == DataGridEditState.None && (object)ParentDataGrid.SelectedRow == (object)Item ) || ( ParentDataGrid.SelectedRows != null && ParentDataGrid.SelectedRows.Any( x => (object)( x ) == (object)Item ) );

        /// <summary>
        /// Gets the row background color.
        /// </summary>
        protected Background GetBackground( DataGridRowStyling styling, DataGridRowStyling selectedStyling ) => IsSelected
            ? ( selectedStyling?.Background ?? Blazorise.Background.Primary )
            : ( styling?.Background ?? Blazorise.Background.None );

        /// <summary>
        /// Gets the row color.
        /// </summary>
        protected Color GetColor( DataGridRowStyling styling, DataGridRowStyling selectedStyling ) => ( IsSelected
            ? selectedStyling?.Color
            : styling?.Color ) ?? Blazorise.Color.None;

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

        [CascadingParameter] protected DataGrid<TItem> ParentDataGrid { get; set; }

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
        [Parameter] public EventCallback<(bool, TItem)> MultiSelect { get; set; }

        /// <summary>
        /// Gets or sets the applied cursor when the row is hovered over.
        /// </summary>
        [Parameter] public Cursor HoverCursor { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}