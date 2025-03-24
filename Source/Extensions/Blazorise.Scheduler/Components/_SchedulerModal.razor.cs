#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Scheduler.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler.Components;

public partial class _SchedulerModal<TItem>
{
    #region Members

    private Modal modalRef;

    private Func<TItem, object> getIdValue;
    private Action<TItem, object> setIdValue;

    private Func<TItem, object> getTitleValue;
    private Action<TItem, object> setTitleValue;

    private Func<TItem, object> getDescriptionValue;
    private Action<TItem, object> setDescriptionValue;

    private Func<TItem, object> getStartValue;
    private Action<TItem, object> setStartValue;

    private Func<TItem, object> getEndValue;
    private Action<TItem, object> setEndValue;

    private readonly Lazy<Func<TItem>> newItemCreator;

    private Validations validationsRef;

    private List<string> customValidationErrors = new();

    #endregion

    #region Constructors

    public _SchedulerModal()
    {
        newItemCreator = new( () => SchedulerFunctionCompiler.CreateNewItem<TItem>() );
    }

    #endregion

    #region Methods

    protected override void OnInitialized()
    {
        if ( typeof( TItem ).GetProperty( IdField ).PropertyType is not null )
        {
            getIdValue = SchedulerFunctionCompiler.CreateValueGetter<TItem>( IdField );
            setIdValue = SchedulerFunctionCompiler.CreateValueSetter<TItem>( IdField );
        }

        if ( typeof( TItem ).GetProperty( TitleField ).PropertyType is not null )
        {
            getTitleValue = SchedulerFunctionCompiler.CreateValueGetter<TItem>( TitleField );
            setTitleValue = SchedulerFunctionCompiler.CreateValueSetter<TItem>( TitleField );
        }

        if ( typeof( TItem ).GetProperty( DescriptionField ).PropertyType is not null )
        {
            getDescriptionValue = SchedulerFunctionCompiler.CreateValueGetter<TItem>( DescriptionField );
            setDescriptionValue = SchedulerFunctionCompiler.CreateValueSetter<TItem>( DescriptionField );
        }

        if ( typeof( TItem ).GetProperty( StartField ).PropertyType is not null )
        {
            getStartValue = SchedulerFunctionCompiler.CreateValueGetter<TItem>( StartField );
            setStartValue = SchedulerFunctionCompiler.CreateValueSetter<TItem>( StartField );
        }

        if ( typeof( TItem ).GetProperty( EndField ).PropertyType is not null )
        {
            getEndValue = SchedulerFunctionCompiler.CreateValueGetter<TItem>( EndField );
            setEndValue = SchedulerFunctionCompiler.CreateValueSetter<TItem>( EndField );
        }

        base.OnInitialized();
    }

    private Task OnStartDateChanged( DateOnly value )
    {
        StartDate = value;

        if ( StartDate > EndDate )
        {
            EndDate = StartDate;
        }

        return Task.CompletedTask;
    }

    private Task OnStartTimeChanged( TimeOnly value )
    {
        var diff = value - StartTime;

        StartTime = value;

        EndTime = EndTime.Add( diff );

        return Task.CompletedTask;
    }

    public Task ShowModal( TItem item, bool isNewItem )
    {
        customValidationErrors.Clear();

        IsNewItem = isNewItem;

        if ( item is null )
            EditItem = newItemCreator.Value();
        else
            EditItem = item;

        Title = getTitleValue?.Invoke( EditItem )?.ToString();
        Description = getDescriptionValue?.Invoke( EditItem )?.ToString();

        if ( StartAvailable )
        {
            var start = (DateTime?)getStartValue?.Invoke( EditItem );
            StartDate = DateOnly.FromDateTime( start.Value );
            StartTime = TimeOnly.FromDateTime( start.Value );
        }

        if ( EndAvailable )
        {
            var end = (DateTime?)getEndValue?.Invoke( EditItem );
            EndDate = DateOnly.FromDateTime( end.Value );
            EndTime = TimeOnly.FromDateTime( end.Value );
        }

        return modalRef.Show();
    }

    public async Task Cancel()
    {
        customValidationErrors.Clear();

        await modalRef.Hide();
    }

    public async Task Submit()
    {
        customValidationErrors.Clear();

        if ( await validationsRef.ValidateAll() )
        {
            if ( EditItem is not null )
            {
                var id = getIdValue?.Invoke( EditItem );

                if ( id is null )
                    setIdValue?.Invoke( EditItem, Guid.NewGuid().ToString() );

                setTitleValue?.Invoke( EditItem, Title );
                setDescriptionValue?.Invoke( EditItem, Description );

                var start = new DateTime( StartDate.Year, StartDate.Month, StartDate.Day, StartTime.Hour, StartTime.Minute, 0 );
                var end = new DateTime( EndDate.Year, EndDate.Month, EndDate.Day, EndTime.Hour, EndTime.Minute, 0 );

                if ( start >= end )
                {
                    customValidationErrors.Add( "The start date must be before the end date." );

                    return;
                }

                setStartValue?.Invoke( EditItem, start );
                setEndValue?.Invoke( EditItem, end );

                var result = await Submited.Invoke( EditItem );

                if ( result )
                {
                    await modalRef.Hide();
                }
            }
        }
    }

    #endregion

    #region Properties

    protected bool IsNewItem { get; set; }

    protected bool TitleAvailable => getTitleValue is not null;

    protected bool DescriptionAvailable => getDescriptionValue is not null;

    protected bool StartAvailable => getStartValue is not null;

    protected bool EndAvailable => getEndValue is not null;

    protected string Title { get; set; }

    protected string Description { get; set; }

    protected DateOnly StartDate { get; set; }

    protected TimeOnly StartTime { get; set; }

    protected DateOnly EndDate { get; set; }

    protected TimeOnly EndTime { get; set; }

    public TItem EditItem { get; set; }

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the unique identifier of the appointment. Defaults to "Id".
    /// </summary>
    [Parameter] public string IdField { get; set; }

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the start date of the appointment. Defaults to "Start".
    /// </summary>
    [Parameter] public string StartField { get; set; }

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the end date of the appointment. Defaults to "End".
    /// </summary>
    [Parameter] public string EndField { get; set; }

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the title of the appointment. Defaults to "Title".
    /// </summary>
    [Parameter] public string TitleField { get; set; }

    /// <summary>
    /// Defines the field name of the <see cref="Scheduler{TItem}"/> that represents the description of the appointment. Defaults to "Description".
    /// </summary>
    [Parameter] public string DescriptionField { get; set; }

    /// <summary>
    /// Represents an event callback that is triggered when an item is saved.
    /// </summary>
    [Parameter] public Func<TItem, Task<bool>> Submited { get; set; }

    #endregion
}
