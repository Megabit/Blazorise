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

    public object GetId( TItem item )
    {
        return getIdFunc?.Invoke( item );
    }

    public void SetId( TItem item, object value )
    {
        setIdFunc?.Invoke( item, value );
    }

    public string GetTitle( TItem item )
    {
        return getTitleFunc?.Invoke( item );
    }

    public void SetTitle( TItem item, string value )
    {
        setTitleFunc?.Invoke( item, value );
    }

    public string GetDescription( TItem item )
    {
        return getDescriptionFunc?.Invoke( item );
    }

    public void SetDescription( TItem item, string value )
    {
        setDescriptionFunc?.Invoke( item, value );
    }

    public DateTime GetStart( TItem item )
    {
        return getStartFunc?.Invoke( item ) ?? DateTime.MinValue;
    }

    public void SetStart( TItem item, DateTime value )
    {
        setStartFunc?.Invoke( item, value );
    }

    public DateTime GetEnd( TItem item )
    {
        return getEndFunc?.Invoke( item ) ?? DateTime.MinValue;
    }

    public void SetEnd( TItem item, DateTime value )
    {
        setEndFunc?.Invoke( item, value );
    }

    public bool GetAllDay( TItem item )
    {
        return getAllDayFunc?.Invoke( item ) ?? false;
    }

    public void SetAllDay( TItem item, bool value )
    {
        setAllDayFunc?.Invoke( item, value );
    }

    public string GetRecurrenceRule( TItem item )
    {
        return getRecurrenceRuleFunc?.Invoke( item );
    }

    public void SetRecurrenceRule( TItem item, string value )
    {
        setRecurrenceRuleFunc?.Invoke( item, value );
    }

    public object GetRecurrenceId( TItem item )
    {
        return getRecurrenceIdFunc?.Invoke( item );
    }

    public void SetRecurrenceId( TItem item, object value )
    {
        setRecurrenceIdFunc?.Invoke( item, value );
    }

    public List<DateTime> GetDeletedOccurrences( TItem item )
    {
        return getDeletedOccurrencesFunc?.Invoke( item );
    }

    public void SetDeletedOccurrences( TItem item, List<DateTime> value )
    {
        setDeletedOccurrencesFunc?.Invoke( item, value );
    }

    public DateTime? GetOriginalStart( TItem item )
    {
        return getOriginalStartFunc?.Invoke( item );
    }

    public void SetOriginalStart( TItem item, DateTime? value )
    {
        setOriginalStartFunc?.Invoke( item, value );
    }

    public List<TItem> GetRecurrenceExceptions( TItem item )
    {
        return getRecurrenceExceptionsFunc?.Invoke( item );
    }

    public void SetRecurrenceExceptions( TItem item, List<TItem> value )
    {
        setRecurrenceExceptionsFunc?.Invoke( item, value );
    }

    public bool HasId => getIdFunc is not null;
    public bool HasTitle => getTitleFunc is not null;
    public bool HasDescription => getDescriptionFunc is not null;
    public bool HasStart => getStartFunc is not null;
    public bool HasEnd => getEndFunc is not null;
    public bool HasAllDay => getAllDayFunc is not null;
    public bool HasRecurrenceRule => getRecurrenceRuleFunc is not null;
    public bool HasRecurrenceId => getRecurrenceIdFunc is not null;
    public bool HasDeletedOccurrences => getDeletedOccurrencesFunc is not null;
    public bool HasOriginalStart => getOriginalStartFunc is not null;
    public bool HasRecurrenceExceptions => getRecurrenceExceptionsFunc is not null;
}