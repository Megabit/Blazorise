#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Blazorise.PivotGrid;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class PivotGridComponentTest : BunitContext
{
    public PivotGridComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop
            .AddBlazoriseModal()
            .AddBlazoriseDragDrop()
            .AddBlazoriseTable();
    }

    [Fact]
    public void Data_Should_Render_AggregatedValues()
    {
        List<PivotSale> sales =
        [
            new() { City = "Berlin", Category = "Hardware", Amount = 10m },
            new() { City = "Berlin", Category = "Hardware", Amount = 20m },
            new() { City = "Paris", Category = "Software", Amount = 40m },
        ];

        IRenderedComponent<PivotGrid<PivotSale>> comp = RenderPivotGrid( parameters =>
        {
            parameters.Add( x => x.Data, sales );
            parameters.Add( x => x.ShowRowTotals, false );
            parameters.Add( x => x.ShowColumnTotals, false );
        } );

        comp.WaitForAssertion( () =>
        {
            IReadOnlyList<string> cellTexts = GetTableCellTexts( comp );

            cellTexts.Should().Contain( "30" );
            cellTexts.Should().Contain( "40" );
        } );
    }

    [Fact]
    public void ReadData_Should_Reload_When_CallbackDelegateChanges()
    {
        EventCallback<PivotGridReadDataEventArgs<PivotSale>> firstReadData = EventCallback.Factory.Create<PivotGridReadDataEventArgs<PivotSale>>(
            this,
            eventArgs =>
            {
                eventArgs.Data =
                [
                    new() { City = "Berlin", Category = "Hardware", Amount = 10m },
                ];

                return Task.CompletedTask;
            } );

        EventCallback<PivotGridReadDataEventArgs<PivotSale>> secondReadData = EventCallback.Factory.Create<PivotGridReadDataEventArgs<PivotSale>>(
            this,
            eventArgs =>
            {
                eventArgs.Data =
                [
                    new() { City = "Paris", Category = "Hardware", Amount = 25m },
                ];

                return Task.CompletedTask;
            } );

        IRenderedComponent<PivotGrid<PivotSale>> comp = RenderPivotGrid( parameters =>
        {
            parameters.Add( x => x.ReadData, firstReadData );
            parameters.Add( x => x.ShowRowTotals, false );
            parameters.Add( x => x.ShowColumnTotals, false );
        } );

        comp.WaitForAssertion( () => GetTableCellTexts( comp ).Should().Contain( "10" ) );

        comp.Render( parameters => parameters.Add( x => x.ReadData, secondReadData ) );

        comp.WaitForAssertion( () =>
        {
            IReadOnlyList<string> cellTexts = GetTableCellTexts( comp );

            cellTexts.Should().Contain( "25" );
            cellTexts.Should().NotContain( "10" );
        } );
    }

    [Fact]
    public void ReadData_Should_Use_ProviderFilterOptions()
    {
        IReadOnlyList<PivotGridFilterOption> cityFilterOptions =
        [
            new( "berlin", "Berlin" ),
            new( "paris", "Paris" ),
        ];

        EventCallback<PivotGridReadDataEventArgs<PivotSale>> readData = EventCallback.Factory.Create<PivotGridReadDataEventArgs<PivotSale>>(
            this,
            eventArgs =>
            {
                eventArgs.Result = new( [], [], [], [], [] );
                eventArgs.FilterOptions = new Dictionary<string, IReadOnlyList<PivotGridFilterOption>>
                {
                    [nameof( PivotSale.City )] = cityFilterOptions,
                };

                return Task.CompletedTask;
            } );

        IRenderedComponent<PivotGrid<PivotSale>> comp = RenderPivotGrid( parameters =>
        {
            parameters.Add( x => x.ReadData, readData );
            parameters.Add( x => x.FieldChooser, true );
        } );

        comp.WaitForAssertion( () =>
        {
            IReadOnlyList<PivotGridFilterOption> filterOptions = GetFilterOptions( comp.Instance, nameof( PivotSale.City ) );

            filterOptions.Select( x => x.Text ).Should().Equal( "Berlin", "Paris" );
        } );
    }

    private IRenderedComponent<PivotGrid<PivotSale>> RenderPivotGrid( Action<ComponentParameterCollectionBuilder<PivotGrid<PivotSale>>> configure )
        => Render<PivotGrid<PivotSale>>( parameters =>
        {
            configure( parameters );
            parameters.Add( x => x.ChildContent, CreatePivotGridFields() );
        } );

    private static IReadOnlyList<string> GetTableCellTexts( IRenderedComponent<PivotGrid<PivotSale>> comp )
        => comp.FindAll( "tbody td" )
            .Select( x => x.TextContent.Trim() )
            .Where( x => !string.IsNullOrWhiteSpace( x ) )
            .ToList();

    private static IReadOnlyList<PivotGridFilterOption> GetFilterOptions( PivotGrid<PivotSale> pivotGrid, string field )
    {
        MethodInfo method = typeof( PivotGrid<PivotSale> ).GetMethod( "GetFilterOptions", BindingFlags.Instance | BindingFlags.NonPublic );

        method.Should().NotBeNull();

        return (IReadOnlyList<PivotGridFilterOption>)method.Invoke( pivotGrid, [new PivotGridFieldState { Field = field }] );
    }

    private static RenderFragment CreatePivotGridFields()
        => builder =>
        {
            builder.OpenComponent<PivotGridFields<PivotSale>>( 0 );
            builder.AddAttribute( 1, nameof( PivotGridFields<PivotSale>.ChildContent ), CreateAvailableFields() );
            builder.CloseComponent();

            builder.OpenComponent<PivotGridColumns<PivotSale>>( 2 );
            builder.AddAttribute( 3, nameof( PivotGridColumns<PivotSale>.ChildContent ), CreateColumnFields() );
            builder.CloseComponent();

            builder.OpenComponent<PivotGridRows<PivotSale>>( 4 );
            builder.AddAttribute( 5, nameof( PivotGridRows<PivotSale>.ChildContent ), CreateRowFields() );
            builder.CloseComponent();

            builder.OpenComponent<PivotGridAggregates<PivotSale>>( 6 );
            builder.AddAttribute( 7, nameof( PivotGridAggregates<PivotSale>.ChildContent ), CreateAggregateFields() );
            builder.CloseComponent();
        };

    private static RenderFragment CreateAvailableFields()
        => builder =>
        {
            AddField<PivotGridField<PivotSale>>( builder, nameof( PivotSale.City ), "City" );
            AddField<PivotGridField<PivotSale>>( builder, nameof( PivotSale.Category ), "Category" );
            AddField<PivotGridField<PivotSale>>( builder, nameof( PivotSale.Amount ), "Amount" );
        };

    private static RenderFragment CreateColumnFields()
        => builder =>
        {
            AddField<PivotGridColumn<PivotSale>>( builder, nameof( PivotSale.City ), "City" );
        };

    private static RenderFragment CreateRowFields()
        => builder =>
        {
            AddField<PivotGridRow<PivotSale>>( builder, nameof( PivotSale.Category ), "Category" );
        };

    private static RenderFragment CreateAggregateFields()
        => builder =>
        {
            AddField<PivotGridAggregate<PivotSale>>( builder, nameof( PivotSale.Amount ), "Amount" );
        };

    private static void AddField<TField>( RenderTreeBuilder builder, string field, string caption )
        where TField : BasePivotGridField<PivotSale>
    {
        builder.OpenComponent<TField>( 0 );
        builder.AddAttribute( 1, nameof( BasePivotGridField<PivotSale>.Field ), field );
        builder.AddAttribute( 2, nameof( BasePivotGridField<PivotSale>.Caption ), caption );
        builder.CloseComponent();
    }

    public class PivotSale
    {
        public string City { get; set; }

        public string Category { get; set; }

        public decimal Amount { get; set; }
    }
}