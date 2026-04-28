#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.PivotGrid.Components;

/// <summary>
/// Internal PivotGrid field chooser modal.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public partial class _PivotGridFieldChooser<TItem>
{
    #region Members

    private const string AvailableZoneName = "available";
    private const string RowsZoneName = "rows";
    private const string ColumnsZoneName = "columns";
    private const string AggregatesZoneName = "aggregates";
    private const string FiltersZoneName = "filters";

    private static readonly IReadOnlyList<PivotGridAggregateFunction> AggregateFunctions = Enum.GetValues<PivotGridAggregateFunction>();

    private Modal modalRef;
    private DropContainer<PivotGridFieldChooserItem> dropContainerRef;
    private List<PivotGridFieldChooserItem> chooserItems = new();
    private int chooserItemIndex;
    private bool refreshDropContainerAfterRender;

    #endregion

    #region Methods

    public async Task Show()
    {
        chooserItemIndex = 0;
        chooserItems = new();

        foreach ( PivotGridFieldState field in PivotGrid.GetFieldChooserCatalog() )
        {
            chooserItems.Add( CreateChooserItem( field, PivotGridFieldArea.Available, true ) );
        }

        AddRuntimeItems( PivotGrid.GetRuntimeRows(), PivotGridFieldArea.Row );
        AddRuntimeItems( PivotGrid.GetRuntimeColumns(), PivotGridFieldArea.Column );
        AddRuntimeItems( PivotGrid.GetRuntimeAggregates(), PivotGridFieldArea.Aggregate );
        AddRuntimeItems( PivotGrid.GetRuntimeFilters(), PivotGridFieldArea.Filter );

        refreshDropContainerAfterRender = true;

        await modalRef.Show();
        await InvokeAsync( StateHasChanged );
    }

    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        if ( refreshDropContainerAfterRender && dropContainerRef is not null )
        {
            refreshDropContainerAfterRender = false;
            dropContainerRef.Refresh();
        }

        return base.OnAfterRenderAsync( firstRender );
    }

    private Task Cancel()
        => modalRef.Hide();

    private async Task Apply()
    {
        PivotGrid.ApplyFieldChooserState(
            GetActiveFieldStates( PivotGridFieldArea.Row ),
            GetActiveFieldStates( PivotGridFieldArea.Column ),
            GetActiveFieldStates( PivotGridFieldArea.Aggregate ),
            GetActiveFieldStates( PivotGridFieldArea.Filter ) );

        await modalRef.Hide();
    }

    private void AddRuntimeItems( IEnumerable<PivotGridFieldState> fields, PivotGridFieldArea area )
    {
        foreach ( PivotGridFieldState field in fields )
        {
            chooserItems.Add( CreateChooserItem( field, area, false ) );
        }
    }

    private PivotGridFieldChooserItem CreateChooserItem( PivotGridFieldState source, PivotGridFieldArea area, bool isAvailableField )
        => new()
        {
            Key = isAvailableField
                ? $"catalog:{source.Field}"
                : $"active:{++chooserItemIndex}:{source.Field}:{area}",
            Field = source.Field,
            Caption = source.Caption,
            FieldType = source.FieldType,
            Area = area,
            AggregateFunction = source.AggregateFunction,
            FilterValueKey = source.FilterValueKey,
            IsAvailableField = isAvailableField,
        };

    private List<PivotGridFieldState> GetActiveFieldStates( PivotGridFieldArea area )
        => chooserItems
            .Where( x => !x.IsAvailableField && x.Area == area )
            .Select( x => PivotGrid<TItem>.CloneFieldState( x ) )
            .ToList();

    private bool FieldChooserItemFilter( PivotGridFieldChooserItem item, string dropZoneName )
        => string.Equals( GetZoneName( item.Area ), dropZoneName, StringComparison.Ordinal );

    private bool CanDrop( PivotGridFieldChooserItem item, string dropZoneName )
    {
        PivotGridFieldArea area = GetArea( dropZoneName );

        if ( area == PivotGridFieldArea.Available )
            return true;

        if ( item.IsAvailableField )
            return !ContainsActiveField( area, item.Field );

        return item.Area == area || !ContainsActiveField( area, item.Field );
    }

    private Task OnItemDropped( DraggableDroppedEventArgs<PivotGridFieldChooserItem> eventArgs )
    {
        PivotGridFieldArea destinationArea = GetArea( eventArgs.DropZoneName );

        if ( eventArgs.Item.IsAvailableField )
        {
            if ( destinationArea != PivotGridFieldArea.Available )
            {
                PivotGridFieldChooserItem clone = CreateChooserItem( eventArgs.Item, destinationArea, false );
                clone.FilterValueKey = string.Empty;
                InsertActiveItem( clone, destinationArea, eventArgs.IndexInZone );
            }
        }
        else if ( destinationArea == PivotGridFieldArea.Available )
        {
            chooserItems.Remove( eventArgs.Item );
        }
        else
        {
            if ( destinationArea != PivotGridFieldArea.Filter )
            {
                eventArgs.Item.FilterValueKey = string.Empty;
            }

            InsertActiveItem( eventArgs.Item, destinationArea, eventArgs.IndexInZone );
        }

        dropContainerRef?.Refresh();

        return Task.CompletedTask;
    }

    private void InsertActiveItem( PivotGridFieldChooserItem item, PivotGridFieldArea area, int indexInZone )
    {
        chooserItems.Remove( item );
        item.Area = area;

        List<PivotGridFieldChooserItem> destinationItems = chooserItems
            .Where( x => !x.IsAvailableField && x.Area == area )
            .ToList();

        if ( indexInZone >= 0 && indexInZone < destinationItems.Count )
        {
            int insertIndex = chooserItems.IndexOf( destinationItems[indexInZone] );
            chooserItems.Insert( insertIndex, item );
        }
        else
        {
            PivotGridFieldChooserItem lastDestinationItem = destinationItems.LastOrDefault();

            if ( lastDestinationItem is null )
            {
                chooserItems.Add( item );
            }
            else
            {
                int insertIndex = chooserItems.IndexOf( lastDestinationItem ) + 1;
                chooserItems.Insert( insertIndex, item );
            }
        }
    }

    private bool ContainsActiveField( PivotGridFieldArea area, string fieldName )
        => chooserItems.Any( x => !x.IsAvailableField && x.Area == area && string.Equals( x.Field, fieldName, StringComparison.Ordinal ) );

    private bool HasItems( PivotGridFieldArea area )
        => chooserItems.Any( x => !x.IsAvailableField && x.Area == area );

    private bool IsAvailableFieldChecked( PivotGridFieldChooserItem field )
        => chooserItems.Any( x => !x.IsAvailableField && string.Equals( x.Field, field.Field, StringComparison.Ordinal ) );

    private Task ToggleAvailableField( PivotGridFieldChooserItem field, bool isChecked )
    {
        if ( isChecked )
        {
            PivotGridFieldArea destinationArea = IsNumericField( field )
                ? PivotGridFieldArea.Aggregate
                : PivotGridFieldArea.Row;

            if ( !ContainsActiveField( destinationArea, field.Field ) )
            {
                PivotGridFieldChooserItem clone = CreateChooserItem( field, destinationArea, false );
                clone.FilterValueKey = string.Empty;
                InsertActiveItem( clone, destinationArea, -1 );
            }
        }
        else
        {
            chooserItems.RemoveAll( x => !x.IsAvailableField && string.Equals( x.Field, field.Field, StringComparison.Ordinal ) );
        }

        dropContainerRef?.Refresh();

        return InvokeAsync( StateHasChanged );
    }

    private static bool IsNumericField( PivotGridFieldChooserItem field )
    {
        Type fieldType = field.FieldType ?? typeof( object );
        fieldType = Nullable.GetUnderlyingType( fieldType ) ?? fieldType;

        return fieldType == typeof( byte )
            || fieldType == typeof( sbyte )
            || fieldType == typeof( short )
            || fieldType == typeof( ushort )
            || fieldType == typeof( int )
            || fieldType == typeof( uint )
            || fieldType == typeof( long )
            || fieldType == typeof( ulong )
            || fieldType == typeof( float )
            || fieldType == typeof( double )
            || fieldType == typeof( decimal );
    }

    private static void SetAggregateFunction( PivotGridFieldChooserItem field, PivotGridAggregateFunction aggregateFunction )
    {
        field.AggregateFunction = aggregateFunction;
    }

    private static void SetFilterValue( PivotGridFieldChooserItem field, string filterValueKey )
    {
        field.FilterValueKey = filterValueKey;
    }

    private static string GetZoneName( PivotGridFieldArea area )
        => area switch
        {
            PivotGridFieldArea.Row => RowsZoneName,
            PivotGridFieldArea.Column => ColumnsZoneName,
            PivotGridFieldArea.Aggregate => AggregatesZoneName,
            PivotGridFieldArea.Filter => FiltersZoneName,
            _ => AvailableZoneName,
        };

    private static PivotGridFieldArea GetArea( string dropZoneName )
        => dropZoneName switch
        {
            RowsZoneName => PivotGridFieldArea.Row,
            ColumnsZoneName => PivotGridFieldArea.Column,
            AggregatesZoneName => PivotGridFieldArea.Aggregate,
            FiltersZoneName => PivotGridFieldArea.Filter,
            _ => PivotGridFieldArea.Available,
        };

    #endregion

    #region Properties

    /// <summary>
    /// Parent PivotGrid component.
    /// </summary>
    [CascadingParameter] public PivotGrid<TItem> PivotGrid { get; set; }

    #endregion
}