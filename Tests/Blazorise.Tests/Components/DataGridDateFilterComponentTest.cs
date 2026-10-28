#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Bootstrap;
using Blazorise.DataGrid;
using Blazorise.DataGrid.Utils;
using Blazorise.Tests.TestServices;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class DataGridDateFilterComponentTest : BunitContext
{
    public DataGridDateFilterComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseDataGrid();
        JSInterop.AddBlazoriseUtilities();
    }

    public static IEnumerable<object[]> DateOnlyFilters()
    {
        var fields = new[]
        {
            nameof( Item.Timestamp ),
            nameof( Item.OffsetTimestamp ),
            nameof( Item.Date ),
            nameof( Item.RequiredTimestamp ),
            nameof( Item.RequiredOffsetTimestamp ),
            nameof( Item.RequiredDate ),
        };

        var filters = new[]
        {
            (DataGridColumnFilterMethod.Equals, new[] { 2, 3, 4 }),
            (DataGridColumnFilterMethod.NotEquals, new[] { 1, 5 }),
            (DataGridColumnFilterMethod.LessThan, new[] { 1 }),
            (DataGridColumnFilterMethod.LessThanOrEqual, new[] { 1, 2, 3, 4 }),
            (DataGridColumnFilterMethod.GreaterThan, new[] { 5 }),
            (DataGridColumnFilterMethod.GreaterThanOrEqual, new[] { 2, 3, 4, 5 }),
            (DataGridColumnFilterMethod.Between, new[] { 1, 2, 3, 4 }),
        };

        foreach ( var field in fields )
        {
            foreach ( var (method, expectedIds) in filters )
            {
                yield return new object[] { field, method, expectedIds };
            }
        }
    }

    [Theory]
    [MemberData( nameof( DateOnlyFilters ) )]
    public async Task DateOnlyFilters_Should_MatchCalendarDates_InLocalAndQueryableData( string field, DataGridColumnFilterMethod method, int[] expectedIds )
    {
        var property = typeof( Item ).GetProperty( field );
        var items = CreateItems();

        if ( Nullable.GetUnderlyingType( property.PropertyType ) is null )
        {
            items = items.Where( item => item.Timestamp is not null ).ToArray();
        }

        var searchValue = method == DataGridColumnFilterMethod.Between
            ? new[] { property.GetValue( items[0] ), property.GetValue( items[2] ) }
            : property.GetValue( items[2] );

        var comp = Render<DataGrid<Item>>( parameters => parameters
            .Add( x => x.Data, items )
            .Add( x => x.DataGridColumns, CreateColumns( field, method, DateComparisonMode.DateOnly ) )
            .Add( x => x.FilterMode, DataGridFilterMode.Menu )
            .Add( x => x.Filterable, true ) );

        var column = comp.FindComponent<DataGridDateColumn<Item>>().Instance;

        await comp.InvokeAsync( () => column.Filter.SearchValue = searchValue );
        await comp.InvokeAsync( () => comp.Instance.Reload() );

        Assert.Equal( expectedIds, comp.Instance.FilteredData.Select( item => item.Id ) );
        Assert.Same( searchValue, column.Filter.SearchValue );

        var columnInfo = new DataGridColumnInfo( field, searchValue, SortDirection.Default, -1, DataGridColumnType.Date, field, method, property.PropertyType )
        {
            FilterComparisonMode = DateComparisonMode.DateOnly,
        };

        var filteredItems = items.AsQueryable().ApplyDataGridSearch( new[] { columnInfo } );

        Assert.Equal( expectedIds, filteredItems.Select( item => item.Id ) );
    }

    [Fact]
    public async Task DefaultComparison_Should_PreserveTime_And_ChangingMode_Should_RefreshFilter()
    {
        var comp = Render<DataGrid<Item>>( parameters => parameters
            .Add( x => x.Data, CreateItems() )
            .Add( x => x.DataGridColumns, CreateColumns( nameof( Item.Timestamp ), DataGridColumnFilterMethod.Equals ) )
            .Add( x => x.Filterable, true ) );

        var column = comp.FindComponent<DataGridDateColumn<Item>>().Instance;
        var searchValue = new DateTime( 2020, 1, 1 );

        await comp.InvokeAsync( () => column.Filter.SearchValue = searchValue );
        await comp.InvokeAsync( () => comp.Instance.Reload() );

        Assert.Equal( DateComparisonMode.DateTime, column.FilterComparisonMode );
        Assert.Equal( 2, Assert.Single( comp.Instance.FilteredData ).Id );

        var columnInfo = new DataGridColumnInfo( nameof( Item.Timestamp ), searchValue, SortDirection.Default, -1, DataGridColumnType.Date, nameof( Item.Timestamp ), DataGridColumnFilterMethod.Equals, typeof( DateTime? ) );

        Assert.Equal( DateComparisonMode.DateTime, columnInfo.FilterComparisonMode );
        Assert.Equal( 2, Assert.Single( CreateItems().Where( item => item.Timestamp is not null ).AsQueryable().ApplyDataGridSearch( new[] { columnInfo } ) ).Id );

        comp.Render( parameters => parameters.Add( x => x.DataGridColumns, CreateColumns( nameof( Item.Timestamp ), DataGridColumnFilterMethod.Equals, DateComparisonMode.DateOnly ) ) );

        comp.WaitForAssertion( () => Assert.Equal( new[] { 2, 3, 4 }, comp.Instance.FilteredData.Select( item => item.Id ) ) );
    }

    [Fact]
    public async Task CustomFilter_Should_ReceiveOriginalTimestamps_And_TakePrecedence()
    {
        var items = CreateItems();
        var searchValue = items[2].Timestamp;
        var comp = Render<DataGrid<Item>>( parameters => parameters
            .Add( x => x.Data, items )
            .Add( x => x.DataGridColumns, CreateColumns( nameof( Item.Timestamp ), DataGridColumnFilterMethod.Equals, DateComparisonMode.DateOnly, true ) )
            .Add( x => x.Filterable, true ) );

        var column = comp.FindComponent<DataGridDateColumn<Item>>().Instance;

        await comp.InvokeAsync( () => column.Filter.SearchValue = searchValue );
        await comp.InvokeAsync( () => comp.Instance.Reload() );

        Assert.Equal( 3, Assert.Single( comp.Instance.FilteredData ).Id );
        Assert.Equal( searchValue, column.Filter.SearchValue );
    }

    [Fact]
    public async Task ReadData_Should_IncludeDateComparisonMode()
    {
        var requests = new List<DataGridColumnInfo[]>();
        var comp = Render<DataGrid<Item>>( parameters => parameters
            .Add( x => x.Data, CreateItems() )
            .Add( x => x.DataGridColumns, CreateColumns( nameof( Item.Timestamp ), DataGridColumnFilterMethod.Equals, DateComparisonMode.DateOnly ) )
            .Add( x => x.Filterable, true )
            .Add( x => x.ReadData, args => requests.Add( args.Columns.ToArray() ) ) );

        comp.WaitForAssertion( () =>
        {
            Assert.Single( requests );
            Assert.False( comp.Instance.IsLoading );
        } );

        var column = comp.FindComponent<DataGridDateColumn<Item>>().Instance;
        var searchValue = new DateTime( 2020, 1, 1 );

        await comp.InvokeAsync( () => column.Filter.SearchValue = searchValue );
        await comp.InvokeAsync( () => comp.Instance.Reload() );

        var columnInfo = Assert.Single( requests.Last() );

        Assert.Equal( DateComparisonMode.DateOnly, columnInfo.FilterComparisonMode );
        Assert.Equal( searchValue, columnInfo.SearchValue );
        Assert.Equal( new[] { 2, 3, 4 }, CreateItems().AsQueryable().ApplyDataGridSearch( requests.Last() ).Select( item => item.Id ) );
    }

    private static Item[] CreateItems()
        => new[]
        {
            new Item { Id = 1, Timestamp = new DateTime( 2019, 12, 31, 23, 59, 59 ), OffsetHours = -12 },
            new Item { Id = 2, Timestamp = new DateTime( 2020, 1, 1 ), OffsetHours = 14 },
            new Item { Id = 3, Timestamp = new DateTime( 2020, 1, 1, 12, 30, 45 ), OffsetHours = 2 },
            new Item { Id = 4, Timestamp = new DateTime( 2020, 1, 2 ).AddTicks( -1 ), OffsetHours = -12 },
            new Item { Id = 5, Timestamp = new DateTime( 2020, 1, 2 ), OffsetHours = 14 },
            new Item { Id = 6 },
        };

    private static RenderFragment CreateColumns( string field, DataGridColumnFilterMethod method, DateComparisonMode? comparisonMode = null, bool customFilter = false )
        => builder =>
        {
            builder.OpenComponent<DataGridDateColumn<Item>>( 0 );
            builder.AddAttribute( 1, nameof( DataGridDateColumn<Item>.Field ), field );
            builder.AddAttribute( 2, nameof( DataGridDateColumn<Item>.FilterMethod ), method );
            builder.AddAttribute( 3, nameof( DataGridDateColumn<Item>.NativeInputMode ), true );

            if ( comparisonMode is not null )
            {
                builder.AddAttribute( 4, nameof( DataGridDateColumn<Item>.FilterComparisonMode ), comparisonMode.Value );
            }

            if ( customFilter )
            {
                builder.AddAttribute( 5, nameof( DataGridDateColumn<Item>.CustomFilter ), (DataGridColumnCustomFilter)( ( value, search ) => search is null || Equals( value, search ) ) );
            }

            builder.CloseComponent();
        };

    public class Item
    {
        public int Id { get; set; }

        public DateTime? Timestamp { get; set; }

        public int OffsetHours { get; set; }

        public DateTimeOffset? OffsetTimestamp => Timestamp is { } timestamp ? new DateTimeOffset( timestamp, TimeSpan.FromHours( OffsetHours ) ) : null;

        public DateOnly? Date => Timestamp is { } timestamp ? DateOnly.FromDateTime( timestamp ) : null;

        public DateTime RequiredTimestamp => Timestamp.GetValueOrDefault();

        public DateTimeOffset RequiredOffsetTimestamp => OffsetTimestamp.GetValueOrDefault();

        public DateOnly RequiredDate => Date.GetValueOrDefault();
    }
}