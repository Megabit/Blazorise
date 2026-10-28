#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Provides data grid date column behavior within a DataGrid.
/// </summary>
public class DataGridDateColumn<TItem> : DataGridColumn<TItem>
{
    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var filterComparisonModeChanged = parameters.TryGetValue<DateComparisonMode>( nameof( FilterComparisonMode ), out var paramFilterComparisonMode ) && paramFilterComparisonMode != FilterComparisonMode;
        var sortComparisonModeChanged = parameters.TryGetValue<DateComparisonMode>( nameof( SortComparisonMode ), out var paramSortComparisonMode ) && paramSortComparisonMode != SortComparisonMode;
        var groupComparisonModeChanged = parameters.TryGetValue<DateComparisonMode>( nameof( GroupComparisonMode ), out var paramGroupComparisonMode ) && paramGroupComparisonMode != GroupComparisonMode;
        var shouldReload = Rendered && ( filterComparisonModeChanged || sortComparisonModeChanged || groupComparisonModeChanged );

        await base.SetParametersAsync( parameters );

        if ( shouldReload && ParentDataGrid is not null )
        {
            await ParentDataGrid.Reload();
        }
    }

    /// <inheritdoc/>
    internal override DataGridColumnFilterMethod GetDefaultFilterMethod()
        => DataGridColumnFilterMethod.Equals;

    /// <inheritdoc />
    protected internal override object NormalizeValueForSort( object value )
        => base.NormalizeValueForSort( SortComparisonMode == DateComparisonMode.DateOnly ? NormalizeDateValue( value ) : value );

    /// <inheritdoc />
    internal override Func<TItem, object> GetGroupByFunc()
        => GroupBy is not null || GroupComparisonMode != DateComparisonMode.DateOnly
            ? base.GetGroupByFunc()
            : item => NormalizeDateValue( GetValue( item ) );

    private static object NormalizeDateValue( object value )
        => value switch
        {
            DateTime dateTime => DateOnly.FromDateTime( dateTime ),
            DateTimeOffset dateTimeOffset => DateOnly.FromDateTime( dateTimeOffset.Date ),
            _ => value,
        };

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override DataGridColumnType ColumnType
        => DataGridColumnType.Date;

    /// <summary>
    /// Hints at the type of data that might be entered by the user while editing the element or its contents.
    /// </summary>
    [Parameter] public DateInputMode InputMode { get; set; }

    /// <summary>
    /// Specifies whether date filters compare the full value or only its calendar date.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="DateComparisonMode.DateTime"/>. Date-only comparisons ignore time without converting time zones.
    /// Applies to equality, ordering, and range filters. Custom filters receive the original values.
    /// </remarks>
    [Parameter] public DateComparisonMode FilterComparisonMode { get; set; } = DateComparisonMode.DateTime;

    /// <summary>
    /// Specifies whether sorting compares the full value or only its calendar date.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="DateComparisonMode.DateTime"/>. Date-only sorting ignores time without converting time zones.
    /// Values on the same day compare equally. A custom <see cref="DataGridColumn{TItem}.SortComparer"/> takes precedence.
    /// </remarks>
    [Parameter] public DateComparisonMode SortComparisonMode { get; set; } = DateComparisonMode.DateTime;

    /// <summary>
    /// Specifies whether grouping uses the full value or only its calendar date.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="DateComparisonMode.DateTime"/>. Date-only grouping uses <see cref="DateOnly"/> keys without converting time zones.
    /// A custom <see cref="DataGridColumn{TItem}.GroupBy"/> takes precedence.
    /// </remarks>
    [Parameter] public DateComparisonMode GroupComparisonMode { get; set; } = DateComparisonMode.DateTime;

    /// <summary>
    /// The earliest date to accept.
    /// </summary>
    [Parameter] public DateTimeOffset? Min { get; set; }

    /// <summary>
    /// The latest date to accept.
    /// </summary>
    [Parameter] public DateTimeOffset? Max { get; set; }

    /// <summary>
    /// Specifies the first day of the week used for date calculations.
    /// </summary>
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Monday;

    /// <summary>
    /// Specifies the input format mask of the date input.
    /// </summary>
    [Parameter] public string InputFormat { get; set; }

    /// <summary>
    /// Displays time picker in 24 hour mode without AM/PM selection when enabled.
    /// </summary>
    [Parameter] public bool TimeAs24hr { get; set; }

    /// <summary>
    /// List of disabled dates that the user should not be able to pick.
    /// </summary>
    [Parameter] public IEnumerable<object> DisabledDates { get; set; }

    /// <summary>
    /// List of disabled days in a week that the user should not be able to pick.
    /// </summary>
    [Parameter] public IEnumerable<DayOfWeek> DisabledDays { get; set; }

    /// <summary>
    /// Display the calendar in an always-open state with the inline option.
    /// </summary>
    [Parameter] public bool Inline { get; set; }

    /// <summary>
    /// If enabled, it disables the native input on mobile devices.
    /// </summary>
    [Parameter] public bool DisableMobile { get; set; } = true;

    /// <summary>
    /// If enabled, the calendar menu will be positioned as static.
    /// </summary>
    [Parameter] public bool StaticPicker { get; set; }

    /// <summary>
    /// Renders the native based input <see cref="Blazorise.DateInput{TValue}"/> instead of the <see cref="Blazorise.DatePicker{TValue}"/>.
    /// </summary>
    [Parameter] public bool NativeInputMode { get; set; }

    #endregion
}