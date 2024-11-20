﻿#region Using directives
using System;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Internal component for editing the row item cell value.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public partial class _DataGridCellDatePicker<TItem> : ComponentBase
{
    #region Members

    protected string elementId;

    /// <summary>
    /// Value data type.
    /// </summary>
    private Type valueType;

    private string dateDisplayFormat;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        valueType = Column.GetValueType( default );
        elementId = IdGenerator.Generate;

        // this is a woraround for https://github.com/Megabit/Blazorise/issues/5837
        // in 2.0 we need to remove formating as {0:dd.MM.yyyy}, and on support dd.MM.yyyy
        if ( Column.DisplayFormat is not null && Column.DisplayFormat.StartsWith( "{0:" ) && Column.DisplayFormat.EndsWith( "}" ) )
        {
            dateDisplayFormat = Column.DisplayFormat.Substring( 3, Column.DisplayFormat.Length - 4 );
        }

        base.OnInitialized();
    }

    public Task OnCellValueChanged<TValue>( TValue value )
        => CellValueChanged.InvokeAsync( value );

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            if ( ParentDataGrid.IsCellEdit && Column.CellEditing )
            {
                await Task.Yield();
                await Focus();
            }
        }
        await base.OnAfterRenderAsync( firstRender );
    }

    public async Task Focus()
    {
        await JSUtilitiesModule.Focus( default, elementId, true );
    }

    #endregion

    #region Properties

    private RenderFragment DatePickerFragment => builder =>
    {
        var type = typeof( DatePicker<> ).MakeGenericType( new[] { valueType } );

        builder.OpenComponent( 0, type );
        builder.AddAttribute( 1, nameof( DatePicker<object>.Value ), valueType switch
        {
            Type typeDateTime when typeDateTime == typeof( DateTime ) => (DateTime)( CellValue ?? (DateTime)default ),
            Type typeDateTimeNull when typeDateTimeNull == typeof( DateTime? ) => (DateTime?)( CellValue ?? (DateTime?)default ),
            Type typeDateOnly when typeDateOnly == typeof( DateOnly ) => (DateOnly)( CellValue ?? (DateOnly)default ),
            Type typeDateOnlyNull when typeDateOnlyNull == typeof( DateOnly? ) => (DateOnly?)( CellValue ?? (DateOnly?)default ),
            Type typeDateTimeOffset when typeDateTimeOffset == typeof( DateTimeOffset ) => (DateTimeOffset)( CellValue ?? (DateTimeOffset)default ),
            Type typeDateTimeOffsetNull when typeDateTimeOffsetNull == typeof( DateTimeOffset? ) => (DateTimeOffset?)( CellValue ?? (DateTimeOffset?)default ),
            _ => throw new InvalidOperationException( $"Unsupported type {valueType}" )
        } );
        builder.AddAttribute( 2, nameof( DatePicker<object>.ValueChanged ), valueType switch
        {
            Type typeDateTime when typeDateTime == typeof( DateTime ) => EventCallback.Factory.Create<DateTime>( this, ( OnCellValueChanged<DateTime> ) ),
            Type typeDateTimeNull when typeDateTimeNull == typeof( DateTime? ) => EventCallback.Factory.Create<DateTime?>( this, ( OnCellValueChanged<DateTime?> ) ),
            Type typeDateOnly when typeDateOnly == typeof( DateOnly ) => EventCallback.Factory.Create<DateOnly>( this, ( OnCellValueChanged<DateOnly> ) ),
            Type typeDateOnlyNull when typeDateOnlyNull == typeof( DateOnly? ) => EventCallback.Factory.Create<DateOnly?>( this, ( OnCellValueChanged<DateOnly?> ) ),
            Type typeDateTimeOffset when typeDateTimeOffset == typeof( DateTimeOffset ) => EventCallback.Factory.Create<DateTimeOffset>( this, ( OnCellValueChanged<DateTimeOffset> ) ),
            Type typeDateTimeOffsetNull when typeDateTimeOffsetNull == typeof( DateTimeOffset? ) => EventCallback.Factory.Create<DateTimeOffset?>( this, ( OnCellValueChanged<DateTimeOffset?> ) ),
            _ => throw new InvalidOperationException( $"Unsupported type {valueType}" )
        } );
        builder.AddAttribute( 3, nameof( BaseInputComponent<object>.ReadOnly ), Column.Readonly );
        builder.AddAttribute( 4, nameof( DatePicker<object>.Pattern ), Column.ValidationPattern );
        builder.AddAttribute( 5, nameof( DatePicker<object>.InputMode ), Column.InputMode );
        builder.AddAttribute( 6, nameof( DatePicker<object>.Min ), Column.Min );
        builder.AddAttribute( 7, nameof( DatePicker<object>.Max ), Column.Max );
        builder.AddAttribute( 8, nameof( DatePicker<object>.FirstDayOfWeek ), Column.FirstDayOfWeek );
        builder.AddAttribute( 9, nameof( DatePicker<object>.InputFormat ), Column.InputFormat );
        builder.AddAttribute( 10, nameof( DatePicker<object>.DisabledDates ), Column.DisabledDates );
        builder.AddAttribute( 11, nameof( DatePicker<object>.DisabledDays ), Column.DisabledDays );
        builder.AddAttribute( 12, nameof( DatePicker<object>.Inline ), Column.Inline );
        builder.AddAttribute( 13, nameof( DatePicker<object>.DisableMobile ), Column.DisableMobile );
        builder.AddAttribute( 14, nameof( DatePicker<object>.StaticPicker ), Column.StaticPicker );
        builder.AddAttribute( 15, nameof( DatePicker<object>.DisplayFormat ), dateDisplayFormat );
        builder.CloseComponent();
    };

    private RenderFragment DateEditFragment => builder =>
    {
        var type = typeof( DateEdit<> ).MakeGenericType( new[] { valueType } );

        builder.OpenComponent( 0, type );
        builder.AddAttribute( 1, nameof( DateEdit<object>.Value ), valueType switch
        {
            Type typeDateTime when typeDateTime == typeof( DateTime ) => (DateTime)( CellValue ?? (DateTime)default ),
            Type typeDateTimeNull when typeDateTimeNull == typeof( DateTime? ) => (DateTime?)( CellValue ?? (DateTime?)default ),
            Type typeDateOnly when typeDateOnly == typeof( DateOnly ) => (DateOnly)( CellValue ?? (DateOnly)default ),
            Type typeDateOnlyNull when typeDateOnlyNull == typeof( DateOnly? ) => (DateOnly?)( CellValue ?? (DateOnly?)default ),
            Type typeDateTimeOffset when typeDateTimeOffset == typeof( DateTimeOffset ) => (DateTimeOffset)( CellValue ?? (DateTimeOffset)default ),
            Type typeDateTimeOffsetNull when typeDateTimeOffsetNull == typeof( DateTimeOffset? ) => (DateTimeOffset?)( CellValue ?? (DateTimeOffset?)default ),
            _ => throw new InvalidOperationException( $"Unsupported type {valueType}" )
        } );
        builder.AddAttribute( 2, nameof( DateEdit<object>.ValueChanged ), valueType switch
        {
            Type typeDateTime when typeDateTime == typeof( DateTime ) => EventCallback.Factory.Create<DateTime>( this, ( OnCellValueChanged<DateTime> ) ),
            Type typeDateTimeNull when typeDateTimeNull == typeof( DateTime? ) => EventCallback.Factory.Create<DateTime?>( this, ( OnCellValueChanged<DateTime?> ) ),
            Type typeDateOnly when typeDateOnly == typeof( DateOnly ) => EventCallback.Factory.Create<DateOnly>( this, ( OnCellValueChanged<DateOnly> ) ),
            Type typeDateOnlyNull when typeDateOnlyNull == typeof( DateOnly? ) => EventCallback.Factory.Create<DateOnly?>( this, ( OnCellValueChanged<DateOnly?> ) ),
            Type typeDateTimeOffset when typeDateTimeOffset == typeof( DateTimeOffset ) => EventCallback.Factory.Create<DateTimeOffset>( this, ( OnCellValueChanged<DateTimeOffset> ) ),
            Type typeDateTimeOffsetNull when typeDateTimeOffsetNull == typeof( DateTimeOffset? ) => EventCallback.Factory.Create<DateTimeOffset?>( this, ( OnCellValueChanged<DateTimeOffset?> ) ),
            _ => throw new InvalidOperationException( $"Unsupported type {valueType}" )
        } );
        builder.AddAttribute( 3, nameof( BaseInputComponent<object>.ReadOnly ), Column.Readonly );
        builder.AddAttribute( 4, nameof( DateEdit<object>.Pattern ), Column.ValidationPattern );
        builder.AddAttribute( 5, nameof( DateEdit<object>.InputMode ), Column.InputMode );
        builder.AddAttribute( 6, nameof( DateEdit<object>.Min ), Column.Min );
        builder.AddAttribute( 7, nameof( DateEdit<object>.Max ), Column.Max );
        builder.CloseComponent();
    };

    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    [Inject] public IIdGenerator IdGenerator { get; set; }
    /// <summary>
    /// Gets or sets the <see cref="IJSUtilitiesModule"/> instance.
    /// </summary>
    [Inject] public IJSUtilitiesModule JSUtilitiesModule { get; set; }

    /// <summary>
    /// Column that this cell belongs to.
    /// </summary>
    [Parameter] public DataGridDateColumn<TItem> Column { get; set; }

    /// <summary>
    /// Currently editing cell value.
    /// </summary>
    [Parameter] public object CellValue { get; set; }

    /// <summary>
    /// Raises when cell value changes.
    /// </summary>
    [Parameter] public EventCallback<object> CellValueChanged { get; set; }

    #endregion
}