#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Base;
using Blazorise.DataGrid.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid.Base
{
    public abstract class BaseDataGridColumn<TItem> : BaseComponent
    {
        #region Members

        private readonly Lazy<Func<Type>> valueTypeGetter;
        private readonly Lazy<Func<object>> defaultValueByType;
        private readonly Lazy<Func<TItem, object>> valueGetter;
        private readonly Lazy<Action<TItem, object>> valueSetter;

        #endregion

        #region Constructors

        public BaseDataGridColumn()
        {
            // TODO: move this to cached FunctionCompiler so it doesn't get compiled every time
            valueTypeGetter = new Lazy<Func<Type>>( () => FunctionCompiler.CreateValueTypeGetter<TItem>( Field ) );
            defaultValueByType = new Lazy<Func<object>>( () => FunctionCompiler.CreateDefaultValueByType<TItem>( Field ) );
            valueGetter = new Lazy<Func<TItem, object>>( () => FunctionCompiler.CreateValueGetter<TItem>( Field ) );
            valueSetter = new Lazy<Action<TItem, object>>( () => FunctionCompiler.CreateValueSetter<TItem>( Field ) );
        }

        #endregion

        #region Methods

        protected override void OnInit()
        {
            // connect column to the parent datagrid
            ParentDataGrid?.Hook( this );

            base.OnInit();
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

        #endregion

        #region Properties

        /// <summary>
        /// Gets the type of column editor.
        /// </summary>
        public abstract DataGridColumnType ColumnType { get; }

        /// <summary>
        /// To bind a column to a data source field, set this property to the required data field name.
        /// </summary>
        [Parameter] public string Field { get; set; }

        /// <summary>
        /// Gets or sets the column's display caption
        /// </summary>
        [Parameter] public string Caption { get; set; }

        /// <summary>
        /// Filter value for this column.
        /// </summary>
        [Parameter] public FilterContext Filter { get; set; } = new FilterContext();

        /// <summary>
        /// Gets or sets the column's sort direction.
        /// </summary>
        /// <remarks>
        /// Currently only one column can be sorted becaouse of the bug in Mono runtime.
        /// </remarks>
        [Parameter] public SortDirection Direction { get; set; }

        /// <summary>
        /// Gets or sets whether users can edit cell values under this column.
        /// </summary>
        [Parameter] public bool AllowEdit { get; set; }

        /// <summary>
        /// Gets or sets whether end-users can sort data by the column's values.
        /// </summary>
        [Parameter] public bool AllowSort { get; set; } = true;

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
        [Parameter] public bool AllowFilter { get; set; } = true;

        /// <summary>
        /// The width of the column.
        /// </summary>
        [Parameter] public string Width { get; set; }

        /// <summary>
        /// Template for custom cell display formating.
        /// </summary>
        [Parameter] public RenderFragment<TItem> DisplayTemplate { get; set; }

        /// <summary>
        /// Template for custom cell editing.
        /// </summary>
        [Parameter] public RenderFragment<CellEditContext> EditTemplate { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        [CascadingParameter] protected BaseDataGrid<TItem> ParentDataGrid { get; set; }

        #endregion
    }
}
