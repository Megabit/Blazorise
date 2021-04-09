#region Using directives
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public class DataGridAggregate<TItem> : BaseDataGridComponent
    {
        #region Members

        private static readonly Type[] ValidNumericTypes = {
            typeof( decimal ), typeof( long ), typeof( int ), typeof( double ), typeof( float ), typeof( short ), typeof( byte ),
            typeof( decimal? ), typeof( long? ), typeof( int? ), typeof( double? ), typeof( float? ), typeof( short? ), typeof( byte? ),
        };

        private static readonly Type[] ValidBooleanTypes = { typeof( bool ), typeof( bool? ) };

        #endregion

        #region Methods

        /// <summary>
        /// Summary of numeric value.
        /// </summary>
        public static object Sum( IEnumerable<TItem> Data, DataGridColumn<TItem> column )
        {
            if ( !ValidNumericTypes.Contains( column.GetValueType() ) )
                return 0;

            return ( from item in Data
                     let value = column.GetValue( item )
                     select Convert.ToDecimal( value ) ).Sum();
        }

        /// <summary>
        /// Min value of numeric value.
        /// </summary>
        public static object Min( IEnumerable<TItem> Data, DataGridColumn<TItem> column )
        {
            if ( !ValidNumericTypes.Contains( column.GetValueType() ) )
                return 0;

            return ( from item in Data
                     let value = column.GetValue( item )
                     select Convert.ToDecimal( value ) ).Min();
        }

        /// <summary>
        /// Max value of numeric value.
        /// </summary>
        public static object Max( IEnumerable<TItem> Data, DataGridColumn<TItem> column )
        {
            if ( !ValidNumericTypes.Contains( column.GetValueType() ) )
                return 0;

            return ( from item in Data
                     let value = column.GetValue( item )
                     select Convert.ToDecimal( value ) ).Max();
        }

        /// <summary>
        /// Average value of numeric value.
        /// </summary>
        public static object Average( IEnumerable<TItem> Data, DataGridColumn<TItem> column )
        {
            if ( !ValidNumericTypes.Contains( column.GetValueType() ) )
                return 0;

            return ( from item in Data
                     let value = column.GetValue( item )
                     select Convert.ToDecimal( value ) ).Average();
        }

        /// <summary>
        /// Count all values that are not null.
        /// </summary>
        public static object Count( IEnumerable<TItem> Data, DataGridColumn<TItem> column )
        {
            return Data.Count( x => column.GetValue( x ) != null );
        }

        /// <summary>
        /// Count all boolean values that are true.
        /// </summary>
        public static object TrueCount( IEnumerable<TItem> Data, DataGridColumn<TItem> column )
        {
            if ( !ValidBooleanTypes.Contains( column.GetValueType() ) )
                return 0;

            return ( from item in Data
                     let value = Convert.ToBoolean( column.GetValue( item ) )
                     where value
                     select value ).Count();
        }

        /// <summary>
        /// Count all boolean values that are false.
        /// </summary>
        public static object FalseCount( IEnumerable<TItem> Data, DataGridColumn<TItem> column )
        {
            if ( !ValidBooleanTypes.Contains( column.GetValueType() ) )
                return 0;

            return ( from item in Data
                     let value = Convert.ToBoolean( column.GetValue( item ) )
                     where !value
                     select value ).Count();
        }

        protected override void OnInitialized()
        {
            if ( ParentDataGrid != null )
            {
                // connect column to the parent datagrid
                ParentDataGrid.Hook( this );
            }

            base.OnInitialized();
        }

        /// <summary>
        /// Gets the formated display value.
        /// </summary>
        /// <param name="value">Item the contains the value to format.</param>
        /// <returns>Formated display value.</returns>
        internal string FormatDisplayValue( object value )
        {
            if ( DisplayFormat != null )
            {
                return string.Format( DisplayFormatProvider ?? CultureInfo.CurrentCulture, DisplayFormat, value );
            }

            return value?.ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        /// To bind a column to a data source field, set this property to the required data field name.
        /// </summary>
        [Parameter] public string Field { get; set; }


        /// <summary>
        /// Type of aggregate calculation.
        /// </summary>
        /// <see cref="AggregationFunction"/>
        [Parameter]
        public DataGridAggregateType Aggregate
        {
            get
            {
                if ( AggregationFunction == null )
                    return DataGridAggregateType.None;
                if ( AggregationFunction == Sum )
                    return DataGridAggregateType.Sum;
                if ( AggregationFunction == Average )
                    return DataGridAggregateType.Average;
                if ( AggregationFunction == Min )
                    return DataGridAggregateType.Min;
                if ( AggregationFunction == Max )
                    return DataGridAggregateType.Max;
                if ( AggregationFunction == Count )
                    return DataGridAggregateType.Count;
                if ( AggregationFunction == FalseCount )
                    return DataGridAggregateType.FalseCount;
                if ( AggregationFunction == TrueCount )
                    return DataGridAggregateType.TrueCount;
                // other custom functions are not mapped
                throw new InvalidOperationException( "Unable to map custom aggregation function to predefined DataGridAggregateType!" );
            }
            set
            {
                switch ( value )
                {
                    case DataGridAggregateType.None:
                        AggregationFunction = null;
                        break;
                    case DataGridAggregateType.Sum:
                        AggregationFunction = Sum;
                        break;
                    case DataGridAggregateType.Average:
                        AggregationFunction = Average;
                        break;
                    case DataGridAggregateType.Min:
                        AggregationFunction = Min;
                        break;
                    case DataGridAggregateType.Max:
                        AggregationFunction = Max;
                        break;
                    case DataGridAggregateType.Count:
                        AggregationFunction = Count;
                        break;
                    case DataGridAggregateType.FalseCount:
                        AggregationFunction = FalseCount;
                        break;
                    case DataGridAggregateType.TrueCount:
                        AggregationFunction = TrueCount;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException( nameof( Aggregate ), value, "Invalid aggregation type!" );
                }
            }
        }

        /// <summary>
        /// Aggregation calculation.
        /// </summary>
        [Parameter] public Func<IEnumerable<TItem>, DataGridColumn<TItem>, object> AggregationFunction { get; set; }

        /// <summary>
        /// Optional display template for aggregate values.
        /// </summary>
        [Parameter] public RenderFragment<AggregateContext> DisplayTemplate { get; set; }

        /// <summary>
        /// Defines the format for display value.
        /// </summary>
        [Parameter] public string DisplayFormat { get; set; }

        /// <summary>
        /// Defines the format provider info for display value.
        /// </summary>
        [Parameter] public IFormatProvider DisplayFormatProvider { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [CascadingParameter] protected DataGrid<TItem> ParentDataGrid { get; set; }

        #endregion
    }
}
