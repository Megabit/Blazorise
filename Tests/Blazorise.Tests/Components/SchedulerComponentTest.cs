#region Using directives
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Blazorise.Bootstrap;
using Blazorise.Scheduler;
using Blazorise.Tests.bUnit;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class SchedulerComponentTest : BunitContext
{
    public SchedulerComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseScheduler();
    }

    [Fact]
    public void DataParameterChange_ShouldIncrementViewRefreshRevision()
    {
        DateOnly selectedDate = DateOnly.FromDateTime( DateTime.Today );
        List<Appointment> initialData = new()
        {
            new Appointment { Id = "1", Title = "Initial", Start = DateTime.Today.AddHours( 9 ), End = DateTime.Today.AddHours( 10 ) },
        };
        List<Appointment> updatedData = new()
        {
            new Appointment { Id = "2", Title = "Updated", Start = DateTime.Today.AddHours( 11 ), End = DateTime.Today.AddHours( 12 ) },
        };

        IRenderedComponent<Scheduler<Appointment>> component = Render<Scheduler<Appointment>>( parameters => parameters
            .Add( x => x.Date, selectedDate )
            .Add( x => x.Data, initialData ) );

        int initialRevision = GetViewRefreshRevision( component.Instance );

        component.Render( parameters => parameters
            .Add( x => x.Date, selectedDate )
            .Add( x => x.Data, updatedData ) );

        int updatedRevision = GetViewRefreshRevision( component.Instance );

        Assert.True( updatedRevision > initialRevision );
    }

    [Fact]
    public async Task Refresh_ShouldIncrementViewRefreshRevision()
    {
        DateOnly selectedDate = DateOnly.FromDateTime( DateTime.Today );
        List<Appointment> data = new();

        IRenderedComponent<Scheduler<Appointment>> component = Render<Scheduler<Appointment>>( parameters => parameters
            .Add( x => x.Date, selectedDate )
            .Add( x => x.Data, data ) );

        int initialRevision = GetViewRefreshRevision( component.Instance );

        await component.Instance.Refresh();

        component.WaitForAssertion( () => Assert.Equal( initialRevision + 1, GetViewRefreshRevision( component.Instance ) ) );
    }

    [Fact]
    public void MonthViewShowWeekNumbersFalse_ShouldHideWeekNumberColumn()
    {
        DateOnly selectedDate = new( 2024, 1, 15 );
        List<Appointment> data = new();

        IRenderedComponent<Scheduler<Appointment>> component = Render<Scheduler<Appointment>>( parameters => parameters
            .Add( x => x.Date, selectedDate )
            .Add( x => x.Data, data )
            .Add( x => x.SelectedView, SchedulerView.Month )
            .Add( x => x.ShowToolbar, false )
            .Add( x => x.ChildContent, CreateMonthViewContent( false ) ) );

        component.Render( parameters => parameters
            .Add( x => x.Date, selectedDate )
            .Add( x => x.Data, data )
            .Add( x => x.SelectedView, SchedulerView.Month )
            .Add( x => x.ShowToolbar, false )
            .Add( x => x.ChildContent, CreateMonthViewContent( false ) ) );

        Assert.NotEmpty( component.FindAll( ".b-scheduler-month-view" ) );
        Assert.Empty( component.FindAll( ".b-scheduler-weeknumbers-column" ) );
    }

    [Theory]
    [InlineData( null, 30, 30 )]
    [InlineData( null, 15, 15 )]
    [InlineData( 0, 30, 30 )]
    [InlineData( -30, 15, 15 )]
    [InlineData( 60, 30, 60 )]
    [InlineData( 15, 30, 15 )]
    [InlineData( 90, 30, 90 )]
    public async Task SlotClicked_ShouldUseDefaultItemDurationOrSlotDuration( int? durationMinutes, int slotMinutes, int expectedMinutes )
    {
        DateTime start = new( 2024, 1, 15, 23, 30, 0 );
        DateTime end = start.AddMinutes( slotMinutes );
        Appointment newItem = new();
        SchedulerSlotClickedEventArgs clickedSlot = null;

        IRenderedComponent<Scheduler<Appointment>> component = Render<Scheduler<Appointment>>( parameters => parameters
            .Add( x => x.Editable, true )
            .Add( x => x.UseInternalEditing, false )
            .Add( x => x.DefaultItemDuration, durationMinutes.HasValue ? TimeSpan.FromMinutes( durationMinutes.Value ) : null )
            .Add( x => x.NewItemCreator, () => newItem )
            .Add( x => x.SlotClicked, args => clickedSlot = args ) );

        MethodInfo method = typeof( Scheduler<Appointment> ).GetMethod( "NotifySlotClicked", BindingFlags.Instance | BindingFlags.NonPublic )!;

        await component.InvokeAsync( () => (Task)method.Invoke( component.Instance, new object[] { start, end } ) );

        Assert.Equal( start, newItem.Start );
        Assert.Equal( start.AddMinutes( expectedMinutes ), newItem.End );
        Assert.NotNull( clickedSlot );
        Assert.Equal( start, clickedSlot.Start );
        Assert.Equal( end, clickedSlot.End );
    }

    [Theory]
    [InlineData( false )]
    [InlineData( true )]
    public async Task DefaultItemDuration_ShouldPreserveExplicitRangesAndAllDayItems( bool allDay )
    {
        DateTime start = new( 2024, 1, 15 );
        DateTime end = allDay ? start : start.AddHours( 2 );
        Appointment newItem = new();

        IRenderedComponent<Scheduler<Appointment>> component = Render<Scheduler<Appointment>>( parameters => parameters
            .Add( x => x.Editable, true )
            .Add( x => x.UseInternalEditing, false )
            .Add( x => x.DefaultItemDuration, TimeSpan.FromMinutes( 60 ) )
            .Add( x => x.NewItemCreator, () => newItem ) );

        string methodName = allDay ? "NotifyAllDaySlotClicked" : "NotifySlotsSelected";
        object[] arguments = allDay ? new object[] { DateOnly.FromDateTime( start ) } : new object[] { start, end };
        MethodInfo method = typeof( Scheduler<Appointment> ).GetMethod( methodName, BindingFlags.Instance | BindingFlags.NonPublic )!;

        await component.InvokeAsync( () => (Task)method.Invoke( component.Instance, arguments ) );

        Assert.Equal( start, newItem.Start );
        Assert.Equal( end, newItem.End );
        Assert.Equal( allDay, newItem.AllDay );
    }

    private static int GetViewRefreshRevision( Scheduler<Appointment> scheduler )
    {
        FieldInfo fieldInfo = typeof( Scheduler<Appointment> ).GetField( "viewRefreshRevision", BindingFlags.Instance | BindingFlags.NonPublic )!;

        Assert.NotNull( fieldInfo );

        return (int)fieldInfo.GetValue( scheduler );
    }

    private static RenderFragment CreateMonthViewContent( bool showWeekNumbers )
    {
        return builder =>
        {
            builder.OpenComponent<SchedulerViews<Appointment>>( 0 );
            builder.AddAttribute( 1, nameof( SchedulerViews<Appointment>.ChildContent ), (RenderFragment)( childBuilder =>
            {
                childBuilder.OpenComponent<SchedulerMonthView<Appointment>>( 2 );
                childBuilder.AddAttribute( 3, nameof( SchedulerMonthView<Appointment>.ShowWeekNumbers ), showWeekNumbers );
                childBuilder.CloseComponent();
            } ) );
            builder.CloseComponent();
        };
    }

    public class Appointment
    {
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public bool AllDay { get; set; }
    }
}