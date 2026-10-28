#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Bootstrap;
using Blazorise.DataGrid;
using Blazorise.Tests.TestServices;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class DataGridDeferredFilterComponentTest : BunitContext
{
    public DataGridDeferredFilterComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseDataGrid();
    }

    [Theory]
    [InlineData( DataGridFilterMode.Default )]
    [InlineData( DataGridFilterMode.Menu )]
    public async Task LocalFilters_Should_ApplyTogether_And_DeferClearing( DataGridFilterMode filterMode )
    {
        var comp = Render<DataGrid<Item>>( parameters => parameters
            .Add( x => x.Data, CreateItems() )
            .Add( x => x.DataGridColumns, CreateColumns() )
            .Add( x => x.Filterable, true )
            .Add( x => x.FilterMode, filterMode )
            .Add( x => x.FilterApplyMode, DataGridFilterApplyMode.Deferred ) );

        var nameColumn = comp.FindComponent<DataGridColumn<Item>>().Instance;
        var quantityColumn = comp.FindComponent<DataGridNumericColumn<Item>>().Instance;

        await comp.InvokeAsync( () => nameColumn.Filter.TriggerFilterChange( "John" ) );
        await comp.InvokeAsync( () => quantityColumn.Filter.TriggerFilterChange( 20 ) );
        await comp.InvokeAsync( () => comp.Instance.Reload() );

        Assert.Equal( 3, comp.Instance.FilteredData.Count() );

        await comp.InvokeAsync( () => comp.Instance.ApplyFilters() );

        Assert.Equal( 20, Assert.Single( comp.Instance.FilteredData ).Quantity );

        await comp.InvokeAsync( () => comp.Instance.ClearFilter( nameof( Item.Quantity ) ) );
        await comp.InvokeAsync( () => comp.Instance.Sort( nameof( Item.Name ) ) );

        Assert.Single( comp.Instance.FilteredData );

        await comp.InvokeAsync( () => comp.Instance.ApplyFilters() );

        Assert.Equal( 2, comp.Instance.FilteredData.Count() );

        await comp.InvokeAsync( () => comp.Instance.ClearFilter() );

        Assert.Equal( 2, comp.Instance.FilteredData.Count() );

        await comp.InvokeAsync( () => comp.Instance.ApplyFilters() );

        Assert.Equal( 3, comp.Instance.FilteredData.Count() );
    }

    [Fact]
    public async Task ReadData_Should_UseAppliedFilters_And_ApplyWithOneRequestFromFirstPage()
    {
        var requests = new List<(int Page, DataGridColumnInfo[] Columns)>();
        var changedPages = new List<int>();

        var comp = Render<DataGrid<Item>>( parameters => parameters
            .Add( x => x.Data, CreateItems() )
            .Add( x => x.DataGridColumns, CreateColumns() )
            .Add( x => x.Filterable, true )
            .Add( x => x.FilterApplyMode, DataGridFilterApplyMode.Deferred )
            .Add( x => x.ShowPager, true )
            .Add( x => x.Page, 3 )
            .Add( x => x.TotalItems, 30 )
            .Add( x => x.ReadData, args => requests.Add( (args.Page, args.Columns.ToArray()) ) )
            .Add( x => x.PageChanged, page => changedPages.Add( page ) ) );

        comp.WaitForAssertion( () =>
        {
            Assert.Single( requests );
            Assert.False( comp.Instance.IsLoading );
        } );

        var nameColumn = comp.FindComponent<DataGridColumn<Item>>().Instance;
        var quantityColumn = comp.FindComponent<DataGridNumericColumn<Item>>().Instance;

        await comp.InvokeAsync( () => comp.FindComponent<TextInput>().Instance.ValueChanged.InvokeAsync( "John" ) );
        await comp.InvokeAsync( () => quantityColumn.Filter.TriggerFilterChange( 20 ) );

        Assert.Single( requests );

        await comp.InvokeAsync( () => comp.Instance.ApplyFilters() );

        Assert.Equal( 2, requests.Count );
        Assert.Equal( 1, requests.Last().Page );
        Assert.Equal( new[] { 1 }, changedPages );
        Assert.Equal( "John", requests.Last().Columns[0].SearchValue );
        Assert.Equal( 20, requests.Last().Columns[1].SearchValue );

        await comp.InvokeAsync( () => nameColumn.Filter.TriggerFilterChange( "Jane" ) );
        await comp.InvokeAsync( () => quantityColumn.Filter.TriggerFilterChange( 30 ) );
        await comp.InvokeAsync( () => comp.Instance.Sort( nameof( Item.Name ) ) );
        await comp.InvokeAsync( () => comp.Instance.Paginate( "2" ) );

        comp.WaitForAssertion( () =>
        {
            Assert.Equal( 4, requests.Count );
            Assert.False( comp.Instance.IsLoading );
        } );

        Assert.All( requests.Skip( 1 ), request =>
        {
            Assert.Equal( "John", request.Columns[0].SearchValue );
            Assert.Equal( 20, request.Columns[1].SearchValue );
        } );

        await comp.InvokeAsync( () => comp.Instance.ClearFilter() );

        Assert.Equal( 4, requests.Count );

        await comp.InvokeAsync( () => comp.Instance.ApplyFilters() );

        Assert.Equal( 5, requests.Count );
        Assert.Equal( 1, requests.Last().Page );
        Assert.All( requests.Last().Columns, column => Assert.Null( column.SearchValue ) );
    }

    [Fact]
    public async Task MenuFilters_Should_DeferOperators_And_CopyAppliedRanges()
    {
        var comp = Render<DataGrid<Item>>( parameters => parameters
            .Add( x => x.Data, CreateItems() )
            .Add( x => x.DataGridColumns, CreateColumns() )
            .Add( x => x.Filterable, true )
            .Add( x => x.FilterMode, DataGridFilterMode.Menu )
            .Add( x => x.FilterApplyMode, DataGridFilterApplyMode.Deferred ) );

        Assert.DoesNotContain( comp.FindAll( "button" ), button => button.TextContent.Trim() == "Filter" );

        var quantityColumn = comp.FindComponent<DataGridNumericColumn<Item>>().Instance;
        var range = new object[] { 10, 20 };

        await comp.InvokeAsync( () => comp.FindComponents<Select<DataGridColumnFilterMethod>>()[1].Instance.ValueChanged.InvokeAsync( DataGridColumnFilterMethod.Between ) );
        await comp.InvokeAsync( () => quantityColumn.Filter.TriggerFilterChange( range ) );
        await comp.InvokeAsync( () => comp.Instance.ApplyFilters() );

        Assert.Equal( new[] { 10, 20 }, comp.Instance.FilteredData.Select( x => x.Quantity ) );

        range[0] = 20;
        await comp.InvokeAsync( () => comp.Instance.Reload() );

        Assert.Equal( new[] { 10, 20 }, comp.Instance.FilteredData.Select( x => x.Quantity ) );

        await comp.InvokeAsync( () => quantityColumn.Filter.TriggerFilterChange( 30 ) );
        await comp.InvokeAsync( () => comp.FindComponents<Select<DataGridColumnFilterMethod>>()[1].Instance.ValueChanged.InvokeAsync( DataGridColumnFilterMethod.Equals ) );
        await comp.InvokeAsync( () => comp.Instance.Reload() );

        Assert.Equal( new[] { 10, 20 }, comp.Instance.FilteredData.Select( x => x.Quantity ) );

        await comp.InvokeAsync( () => comp.Instance.ApplyFilters() );

        Assert.Equal( 30, Assert.Single( comp.Instance.FilteredData ).Quantity );
    }

    private static Item[] CreateItems()
        => new[]
        {
            new Item { Name = "John", Quantity = 10 },
            new Item { Name = "John", Quantity = 20 },
            new Item { Name = "Jane", Quantity = 30 },
        };

    private static RenderFragment CreateColumns()
        => builder =>
        {
            builder.OpenComponent<DataGridColumn<Item>>( 0 );
            builder.AddAttribute( 1, nameof( DataGridColumn<Item>.Field ), nameof( Item.Name ) );
            builder.CloseComponent();

            builder.OpenComponent<DataGridNumericColumn<Item>>( 2 );
            builder.AddAttribute( 3, nameof( DataGridNumericColumn<Item>.Field ), nameof( Item.Quantity ) );
            builder.AddAttribute( 4, nameof( DataGridNumericColumn<Item>.NativeInputMode ), true );
            builder.CloseComponent();
        };

    public class Item
    {
        public string Name { get; set; }

        public int Quantity { get; set; }
    }
}