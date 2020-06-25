#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public partial class _DataGridAggregateRow<TItem> : BaseDataGridComponent
    {
        #region Members

        private static readonly Type[] ValidNumericTypes = {
            typeof( decimal ), typeof( long ), typeof( int ), typeof( double ), typeof( float ), typeof( short ), typeof( byte ),
            typeof( decimal? ), typeof( long? ), typeof( int? ), typeof( double? ), typeof( float? ), typeof( short? ), typeof( byte? ),
        };

        private static readonly Type[] ValidBooleanTypes = { typeof( bool ), typeof( bool? ) };

        #endregion

        #region Methods

        protected object Calculate( DataGridAggregate<TItem> aggregate, DataGridColumn<TItem> column )
        {
            switch ( aggregate.Aggregate )
            {
                case DataGridAggregateType.Sum:
                    return SumOf( column );
                case DataGridAggregateType.Average:
                    return AverageOf( column );
                case DataGridAggregateType.Min:
                    return MinOf( column );
                case DataGridAggregateType.Max:
                    return MaxOf( column );
                case DataGridAggregateType.Count:
                    return CountOf( column );
                case DataGridAggregateType.TrueCount:
                    return TrueCountOf( column );
                case DataGridAggregateType.FalseCount:
                    return FalseCountOf( column );
                default:
                    return null;
            }
        }

        protected decimal SumOf( DataGridColumn<TItem> column )
        {
            if ( !ValidNumericTypes.Contains( column.GetValueType() ) )
                return 0;

            return ( from item in Data
                     let value = column.GetValue( item )
                     select Convert.ToDecimal( value ) ).Sum();
        }

        protected decimal MinOf( DataGridColumn<TItem> column )
        {
            if ( !ValidNumericTypes.Contains( column.GetValueType() ) )
                return 0;

            return ( from item in Data
                     let value = column.GetValue( item )
                     select Convert.ToDecimal( value ) ).Min();
        }

        protected decimal MaxOf( DataGridColumn<TItem> column )
        {
            if ( !ValidNumericTypes.Contains( column.GetValueType() ) )
                return 0;

            return ( from item in Data
                     let value = column.GetValue( item )
                     select Convert.ToDecimal( value ) ).Max();
        }

        protected decimal AverageOf( DataGridColumn<TItem> column )
        {
            if ( !ValidNumericTypes.Contains( column.GetValueType() ) )
                return 0;

            return ( from item in Data
                     let value = column.GetValue( item )
                     select Convert.ToDecimal( value ) ).Average();
        }

        protected int CountOf( DataGridColumn<TItem> column )
        {
            return Data.Count( x => column.GetValue( x ) != null );
        }

        protected int TrueCountOf( DataGridColumn<TItem> column )
        {
            if ( !ValidBooleanTypes.Contains( column.GetValueType() ) )
                return 0;

            return ( from item in Data
                     let value = Convert.ToBoolean( column.GetValue( item ) )
                     where value
                     select value ).Count();
        }

        protected int FalseCountOf( DataGridColumn<TItem> column )
        {
            if ( !ValidBooleanTypes.Contains( column.GetValueType() ) )
                return 0;

            return ( from item in Data
                     let value = Convert.ToBoolean( column.GetValue( item ) )
                     where !value
                     select value ).Count();
        }

        #endregion

        #region Properties

        protected IEnumerable<TItem> Data
            => ParentDataGrid.ManualReadMode ? ParentDataGrid.AggregateData : ParentDataGrid.Data;

        /// <summary>
        /// List of columns used to build this row.
        /// </summary>
        [Parameter] public IEnumerable<DataGridColumn<TItem>> Columns { get; set; }

        /// <summary>
        /// List of aggregate columns used to build this row.
        /// </summary>
        [Parameter] public IEnumerable<DataGridAggregate<TItem>> Aggregates { get; set; }

        /// <summary>
        /// Custom css classname.
        /// </summary>
        [Parameter] public string Class { get; set; }

        /// <summary>
        /// Custom html style.
        /// </summary>
        [Parameter] public string Style { get; set; }

        /// <summary>
        /// Custom background.
        /// </summary>
        [Parameter] public Background Background { get; set; }

        /// <summary>
        /// Custom color.
        /// </summary>
        [Parameter] public Color Color { get; set; }

        [CascadingParameter] protected DataGrid<TItem> ParentDataGrid { get; set; }

        #endregion
    }
}
