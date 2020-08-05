﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.DataGrid.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public partial class DataGridColumn<TItem> : BaseDataGridColumn<TItem>, IDisposable
    {
        #region Members

        private readonly Lazy<Func<Type>> valueTypeGetter;
        private readonly Lazy<Func<object>> defaultValueByType;
        private readonly Lazy<Func<TItem, object>> valueGetter;
        private readonly Lazy<Action<TItem, object>> valueSetter;

        #endregion

        #region Constructors

        public DataGridColumn()
        {
            // TODO: move this to cached FunctionCompiler so it doesn't get compiled every time
            valueTypeGetter = new Lazy<Func<Type>>( () => FunctionCompiler.CreateValueTypeGetter<TItem>( Field ) );
            defaultValueByType = new Lazy<Func<object>>( () => FunctionCompiler.CreateDefaultValueByType<TItem>( Field ) );
            valueGetter = new Lazy<Func<TItem, object>>( () => FunctionCompiler.CreateValueGetter<TItem>( Field ) );
            valueSetter = new Lazy<Action<TItem, object>>( () => FunctionCompiler.CreateValueSetter<TItem>( Field ) );
        }

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            if ( ParentDataGrid != null )
            {
                // connect column to the parent datagrid
                ParentDataGrid.Hook( this );

                if ( FilterTemplate != null )
                {
                    InitializeFilterContext();
                }
            }

            // initialize temporary variables
            CurrentDirection = Direction;

            base.OnInitialized();
        }

        public void Dispose()
        {
            if ( FilterContext != null )
            {
                FilterContext.Unsubscribe( OnFilterValueChanged );

                FilterContext = null;
            }
        }

        private void InitializeFilterContext()
        {
            FilterContext = new FilterContext
            {
                SearchValue = Filter.SearchValue
            };

            FilterContext.Subscribe( OnFilterValueChanged );
        }

        public async void OnFilterValueChanged( string filterValue )
        {
            await ParentDataGrid.OnFilterChanged( this, filterValue );
        }

        /// <summary>
        /// Gets the typeof() of the value associated with this column field.
        /// </summary>
        /// <returns></returns>
        internal Type GetValueType()
            => valueTypeGetter.Value();

        /// <summary>
        /// Gets default value based on the typeof() of the value associated with this column field.
        /// </summary>
        /// <returns></returns>
        internal object GetDefaultValueByType()
            => defaultValueByType.Value();

        /// <summary>
        /// Gets the current value for the field in the supplied model.
        /// </summary>
        /// <param name="item">Item for which ro set the value.</param>
        /// <returns></returns>
        internal object GetValue( TItem item )
            => valueGetter.Value( item );

        /// <summary>
        /// Sets the value for the field in the supplied model.
        /// </summary>
        /// <param name="item">Item for which ro set the value.</param>
        /// <param name="value">Value to set.</param>
        internal void SetValue( TItem item, object value )
            => valueSetter.Value( item, value );

        public string FormatDisplayValue( TItem item )
        {
            return FormatDisplayValue( GetValue( item ) );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current sort direction.
        /// </summary>
        /// <remarks>
        /// The reason for this field is that <see cref="Direction"/> is reseted every
        /// time when the grid is refreshed by the user.
        /// </remarks>
        internal SortDirection CurrentDirection { get; set; }

        /// <summary>
        /// Gets the type of column editor.
        /// </summary>
        public virtual DataGridColumnType ColumnType { get; } = DataGridColumnType.Text;

        /// <summary>
        /// Gets or sets the column's display caption.
        /// </summary>
        [Parameter] public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the column's display caption template.
        /// </summary>
        [Parameter] public RenderFragment<DataGridColumn<TItem>> CaptionTemplate { get; set; }

        /// <summary>
        /// Filter value for this column.
        /// </summary>
        [Parameter] public FilterContext Filter { get; set; } = new FilterContext();

        /// <summary>
        /// Gets or sets the column initial sort direction.
        /// </summary>
        /// </remarks>
        [Parameter] public SortDirection Direction { get; set; }

        /// <summary>
        /// Defines the alignment for display cell.
        /// </summary>
        [Parameter] public TextAlignment TextAlignment { get; set; }

        /// <summary>
        /// Gets or sets whether users can edit cell values under this column.
        /// </summary>
        [Parameter] public bool Editable { get; set; }

        /// <summary>
        /// Gets or sets whether column can be displayed on a grid.
        /// </summary>
        [Parameter] public bool Displayable { get; set; } = true;

        /// <summary>
        /// Allows the cell values to be entered while the grid is in the new-item state.
        /// </summary>
        [Parameter] public bool CellsEditableOnNewCommand { get; set; } = true;

        /// <summary>
        /// Allows the cell values to be entered while the grid is in the edit-item state.
        /// </summary>
        [Parameter] public bool CellsEditableOnEditCommand { get; set; } = true;

        /// <summary>
        /// Gets or sets whether end-users can sort data by the column's values.
        /// </summary>
        [Parameter] public bool Sortable { get; set; } = true;

        /// <summary>
        /// Gets or sets whether end-users are prevented from editing the column's cell values.
        /// </summary>
        [Parameter] public bool Readonly { get; set; }

        /// <summary>
        /// Gets or sets whether the column's caption is displayed within the column header.
        /// </summary>
        [Parameter] public bool ShowCaption { get; set; } = true;

        /// <summary>
        /// Gets or sets whether users can filter rows by its cell values.
        /// </summary>
        [Parameter] public bool Filterable { get; set; } = true;

        /// <summary>
        /// The width of the column.
        /// </summary>
        [Parameter] public string Width { get; set; }

        /// <summary>
        /// Custom classname handler for cell based on the current row item.
        /// </summary>
        [Parameter] public Func<TItem, string> CellClass { get; set; }

        /// <summary>
        /// Custom style handler for cell based on the current row item.
        /// </summary>
        [Parameter] public Func<TItem, string> CellStyle { get; set; }

        /// <summary>
        /// Custom classname for header cell.
        /// </summary>
        [Parameter] public string HeaderCellClass { get; set; }

        /// <summary>
        /// Custom style for header cell.
        /// </summary>
        [Parameter] public string HeaderCellStyle { get; set; }

        /// <summary>
        /// Custom classname for filter cell.
        /// </summary>
        [Parameter] public string FilterCellClass { get; set; }

        /// <summary>
        /// Custom style for filter cell.
        /// </summary>
        [Parameter] public string FilterCellStyle { get; set; }

        /// <summary>
        /// Custom classname for group cell.
        /// </summary>
        [Parameter] public string GroupCellClass { get; set; }

        /// <summary>
        /// Custom style for group cell.
        /// </summary>
        [Parameter] public string GroupCellStyle { get; set; }

        /// <summary>
        /// Template for custom cell display formating.
        /// </summary>
        [Parameter] public RenderFragment<TItem> DisplayTemplate { get; set; }

        /// <summary>
        /// Template for custom column filter rendering.
        /// </summary>
        [Parameter] public RenderFragment<FilterContext> FilterTemplate { get; set; }

        /// <summary>
        /// Defines the size of field for popup modal.
        /// </summary>
        [Parameter] public IFluentColumn PopupFieldColumnSize { get; set; } = ColumnSize.IsHalf.OnDesktop;

        internal FilterContext FilterContext { get; set; }

        /// <summary>
        /// Template for custom cell editing.
        /// </summary>
        [Parameter] public RenderFragment<CellEditContext> EditTemplate { get; set; }

        /// <summary>
        /// Validates the input value after trying to save.
        /// </summary>
        [Parameter]
        public Action<ValidatorEventArgs> Validator { get; set; }

        /// <summary>
        /// Forces validation to use regex pattern matching instead of default validator handler.
        /// </summary>
        [Parameter] public string ValidationPattern { get; set; }

        #endregion
    }
}
