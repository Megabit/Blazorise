#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Scheduler.Utilities;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Maps properties of a scheduler item, allowing retrieval and modification of various attributes like title, description, and dates.
/// </summary>
/// <typeparam name="TItem">Represents the type of the items being scheduled, enabling flexible property mapping for different item types.</typeparam>
public class SchedulerPropertyMapper<TItem>
{
    private Func<TItem, object> getIdFunc;
    private Action<TItem, object> setIdFunc;

    private Func<TItem, string> getTitleFunc;
    private Action<TItem, string> setTitleFunc;

    private Func<TItem, string> getDescriptionFunc;
    private Action<TItem, string> setDescriptionFunc;

    private Func<TItem, DateTime> getStartFunc;
    private Action<TItem, DateTime> setStartFunc;

    private Func<TItem, DateTime> getEndFunc;
    private Action<TItem, DateTime> setEndFunc;

    private Func<TItem, bool> getAllDayFunc;
    private Action<TItem, bool> setAllDayFunc;

    private Func<TItem, string> getRecurrenceRuleFunc;
    private Action<TItem, string> setRecurrenceRuleFunc;

    private Func<TItem, object> getRecurrenceIdFunc;
    private Action<TItem, object> setRecurrenceIdFunc;

    private Func<TItem, List<DateTime>> getDeletedOccurrencesFunc;
    private Action<TItem, List<DateTime>> setDeletedOccurrencesFunc;

    private Func<TItem, DateTime?> getOriginalStartFunc;
    private Action<TItem, DateTime?> setOriginalStartFunc;

    private Func<TItem, List<TItem>> getRecurrenceExceptionsFunc;
    private Action<TItem, List<TItem>> setRecurrenceExceptionsFunc;

    /// <summary>
    /// Initializes a new instance of the <see cref="SchedulerPropertyMapper{TItem}"/> class,
    /// compiling property accessors and mutators based on the scheduler's field definitions.
    /// </summary>
    /// <param name="scheduler">The scheduler configuration used to determine which properties to map.</param>
    public SchedulerPropertyMapper( Scheduler<TItem> scheduler )
    {
        if ( !string.IsNullOrEmpty( scheduler.IdField ) && typeof( TItem ).GetProperty( scheduler.IdField )?.PropertyType is not null )
        {
            getIdFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem>( scheduler.IdField );
            setIdFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem>( scheduler.IdField );
        }

        if ( !string.IsNullOrEmpty( scheduler.TitleField ) && typeof( TItem ).GetProperty( scheduler.TitleField )?.PropertyType is not null )
        {
            getTitleFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem, string>( scheduler.TitleField );
            setTitleFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem, string>( scheduler.TitleField );
        }

        if ( !string.IsNullOrEmpty( scheduler.DescriptionField ) && typeof( TItem ).GetProperty( scheduler.DescriptionField )?.PropertyType is not null )
        {
            getDescriptionFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem, string>( scheduler.DescriptionField );
            setDescriptionFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem, string>( scheduler.DescriptionField );
        }

        if ( !string.IsNullOrEmpty( scheduler.StartField ) && typeof( TItem ).GetProperty( scheduler.StartField )?.PropertyType is not null )
        {
            getStartFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem, DateTime>( scheduler.StartField );
            setStartFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem, DateTime>( scheduler.StartField );
        }

        if ( !string.IsNullOrEmpty( scheduler.EndField ) && typeof( TItem ).GetProperty( scheduler.EndField )?.PropertyType is not null )
        {
            getEndFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem, DateTime>( scheduler.EndField );
            setEndFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem, DateTime>( scheduler.EndField );
        }

        if ( !string.IsNullOrEmpty( scheduler.AllDayField ) && typeof( TItem ).GetProperty( scheduler.AllDayField )?.PropertyType is not null )
        {
            getAllDayFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem, bool>( scheduler.AllDayField );
            setAllDayFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem, bool>( scheduler.AllDayField );
        }

        if ( !string.IsNullOrEmpty( scheduler.RecurrenceRuleField ) && typeof( TItem ).GetProperty( scheduler.RecurrenceRuleField )?.PropertyType is not null )
        {
            getRecurrenceRuleFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem, string>( scheduler.RecurrenceRuleField );
            setRecurrenceRuleFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem, string>( scheduler.RecurrenceRuleField );
        }

        if ( !string.IsNullOrEmpty( scheduler.RecurrenceIdField ) && typeof( TItem ).GetProperty( scheduler.RecurrenceIdField )?.PropertyType is not null )
        {
            getRecurrenceIdFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem, object>( scheduler.RecurrenceIdField );
            setRecurrenceIdFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem, object>( scheduler.RecurrenceIdField );
        }

        if ( !string.IsNullOrEmpty( scheduler.RecurrenceExceptionsField ) && typeof( TItem ).GetProperty( scheduler.RecurrenceExceptionsField )?.PropertyType is not null )
        {
            getRecurrenceExceptionsFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem, List<TItem>>( scheduler.RecurrenceExceptionsField );
            setRecurrenceExceptionsFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem, List<TItem>>( scheduler.RecurrenceExceptionsField );
        }

        if ( !string.IsNullOrEmpty( scheduler.OriginalStartField ) && typeof( TItem ).GetProperty( scheduler.OriginalStartField )?.PropertyType is not null )
        {
            getOriginalStartFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem, DateTime?>( scheduler.OriginalStartField );
            setOriginalStartFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem, DateTime?>( scheduler.OriginalStartField );
        }

        if ( !string.IsNullOrEmpty( scheduler.DeletedOccurrencesField ) && typeof( TItem ).GetProperty( scheduler.DeletedOccurrencesField )?.PropertyType is not null )
        {
            getDeletedOccurrencesFunc = SchedulerFunctionCompiler.CreateValueGetter<TItem, List<DateTime>>( scheduler.DeletedOccurrencesField );
            setDeletedOccurrencesFunc = SchedulerFunctionCompiler.CreateValueSetter<TItem, List<DateTime>>( scheduler.DeletedOccurrencesField );
        }
    }

    /// <summary>Gets the ID value from the scheduler item.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <returns>The ID value.</returns>
    public object GetId( TItem item ) => getIdFunc?.Invoke( item );

    /// <summary>Sets the ID value on the scheduler item.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <param name="value">The new ID value.</param>
    public void SetId( TItem item, object value ) => setIdFunc?.Invoke( item, value );

    /// <summary>Gets the title from the scheduler item.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <returns>The title string.</returns>
    public string GetTitle( TItem item ) => getTitleFunc?.Invoke( item );

    /// <summary>Sets the title on the scheduler item.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <param name="value">The new title.</param>
    public void SetTitle( TItem item, string value ) => setTitleFunc?.Invoke( item, value );

    /// <summary>Gets the description from the scheduler item.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <returns>The description string.</returns>
    public string GetDescription( TItem item ) => getDescriptionFunc?.Invoke( item );

    /// <summary>Sets the description on the scheduler item.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <param name="value">The new description.</param>
    public void SetDescription( TItem item, string value ) => setDescriptionFunc?.Invoke( item, value );

    /// <summary>Gets the start date and time from the scheduler item.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <returns>The start <see cref="DateTime"/>, or <see cref="DateTime.MinValue"/> if unavailable.</returns>
    public DateTime GetStart( TItem item ) => getStartFunc?.Invoke( item ) ?? DateTime.MinValue;

    /// <summary>Sets the start date and time on the scheduler item.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <param name="value">The new start date and time.</param>
    public void SetStart( TItem item, DateTime value ) => setStartFunc?.Invoke( item, value );

    /// <summary>Gets the end date and time from the scheduler item.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <returns>The end <see cref="DateTime"/>, or <see cref="DateTime.MinValue"/> if unavailable.</returns>
    public DateTime GetEnd( TItem item ) => getEndFunc?.Invoke( item ) ?? DateTime.MinValue;

    /// <summary>Sets the end date and time on the scheduler item.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <param name="value">The new end date and time.</param>
    public void SetEnd( TItem item, DateTime value ) => setEndFunc?.Invoke( item, value );

    /// <summary>Gets whether the scheduler item is marked as an all-day event.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <returns><c>true</c> if all-day; otherwise, <c>false</c>.</returns>
    public bool GetAllDay( TItem item ) => getAllDayFunc?.Invoke( item ) ?? false;

    /// <summary>Sets the all-day flag on the scheduler item.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <param name="value">The all-day flag.</param>
    public void SetAllDay( TItem item, bool value ) => setAllDayFunc?.Invoke( item, value );

    /// <summary>Gets the recurrence rule from the scheduler item.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <returns>The recurrence rule string (RFC 5545 format).</returns>
    public string GetRecurrenceRule( TItem item ) => getRecurrenceRuleFunc?.Invoke( item );

    /// <summary>Sets the recurrence rule on the scheduler item.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <param name="value">The recurrence rule string.</param>
    public void SetRecurrenceRule( TItem item, string value ) => setRecurrenceRuleFunc?.Invoke( item, value );

    /// <summary>Gets the recurrence ID (series ID) from the scheduler item.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <returns>The recurrence ID value.</returns>
    public object GetRecurrenceId( TItem item ) => getRecurrenceIdFunc?.Invoke( item );

    /// <summary>Sets the recurrence ID (series ID) on the scheduler item.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <param name="value">The new recurrence ID.</param>
    public void SetRecurrenceId( TItem item, object value ) => setRecurrenceIdFunc?.Invoke( item, value );

    /// <summary>Gets the list of deleted occurrences from the scheduler item.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <returns>A list of deleted occurrence <see cref="DateTime"/> values.</returns>
    public List<DateTime> GetDeletedOccurrences( TItem item ) => getDeletedOccurrencesFunc?.Invoke( item );

    /// <summary>Sets the list of deleted occurrences on the scheduler item.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <param name="value">A list of deleted occurrence <see cref="DateTime"/> values.</param>
    public void SetDeletedOccurrences( TItem item, List<DateTime> value ) => setDeletedOccurrencesFunc?.Invoke( item, value );

    /// <summary>Gets the original start time from a modified occurrence.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <returns>The original start time, or <c>null</c>.</returns>
    public DateTime? GetOriginalStart( TItem item ) => getOriginalStartFunc?.Invoke( item );

    /// <summary>Sets the original start time on a modified occurrence.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <param name="value">The original start time.</param>
    public void SetOriginalStart( TItem item, DateTime? value ) => setOriginalStartFunc?.Invoke( item, value );

    /// <summary>Gets the recurrence exception items from the scheduler item.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <returns>A list of recurrence exception items.</returns>
    public List<TItem> GetRecurrenceExceptions( TItem item ) => getRecurrenceExceptionsFunc?.Invoke( item );

    /// <summary>Sets the recurrence exception items on the scheduler item.</summary>
    /// <param name="item">The scheduler item.</param>
    /// <param name="value">A list of recurrence exception items.</param>
    public void SetRecurrenceExceptions( TItem item, List<TItem> value ) => setRecurrenceExceptionsFunc?.Invoke( item, value );

    /// <summary>Gets whether the item has an ID property mapped.</summary>
    public bool HasId => getIdFunc is not null;

    /// <summary>Gets whether the item has a Title property mapped.</summary>
    public bool HasTitle => getTitleFunc is not null;

    /// <summary>Gets whether the item has a Description property mapped.</summary>
    public bool HasDescription => getDescriptionFunc is not null;

    /// <summary>Gets whether the item has a Start property mapped.</summary>
    public bool HasStart => getStartFunc is not null;

    /// <summary>Gets whether the item has an End property mapped.</summary>
    public bool HasEnd => getEndFunc is not null;

    /// <summary>Gets whether the item has an AllDay property mapped.</summary>
    public bool HasAllDay => getAllDayFunc is not null;

    /// <summary>Gets whether the item has a RecurrenceRule property mapped.</summary>
    public bool HasRecurrenceRule => getRecurrenceRuleFunc is not null;

    /// <summary>Gets whether the item has a RecurrenceId property mapped.</summary>
    public bool HasRecurrenceId => getRecurrenceIdFunc is not null;

    /// <summary>Gets whether the item has a DeletedOccurrences property mapped.</summary>
    public bool HasDeletedOccurrences => getDeletedOccurrencesFunc is not null;

    /// <summary>Gets whether the item has an OriginalStart property mapped.</summary>
    public bool HasOriginalStart => getOriginalStartFunc is not null;

    /// <summary>Gets whether the item has a RecurrenceExceptions property mapped.</summary>
    public bool HasRecurrenceExceptions => getRecurrenceExceptionsFunc is not null;
}