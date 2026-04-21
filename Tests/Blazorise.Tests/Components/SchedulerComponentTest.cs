#region Using directives
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Blazorise.Bootstrap;
using Blazorise.Scheduler;
using Blazorise.Tests.bUnit;
using Bunit;
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

    private static int GetViewRefreshRevision( Scheduler<Appointment> scheduler )
    {
        FieldInfo fieldInfo = typeof( Scheduler<Appointment> ).GetField( "viewRefreshRevision", BindingFlags.Instance | BindingFlags.NonPublic )!;

        Assert.NotNull( fieldInfo );

        return (int)fieldInfo.GetValue( scheduler );
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