using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Blazorise.DataGrid;

namespace BasicTestApp.Client.PerfComponents;

public partial class DataGridPerf<TItem> : DataGrid<TItem>
{
    public int RenderCalls { get; private set; }

    public int ParametersSetCalls { get; private set; }

    public event EventHandler<PerfEventArgs> PerfEvent;

    public void ResetCalls()
    {
        RenderCalls = 0;
        ParametersSetCalls = 0;
    }

    protected override async Task OnParametersSetAsync()
    {
        ParametersSetCalls++;
        var time = Stopwatch.GetTimestamp();
        await base.OnParametersSetAsync();
        var timeElapsed = Stopwatch.GetElapsedTime( time );
        Console.WriteLine( timeElapsed );
    }

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        RenderCalls++;
        var time = Stopwatch.GetTimestamp();
        await base.OnAfterRenderAsync( firstRender );
        var timeElapsed = Stopwatch.GetElapsedTime( time );
        Console.WriteLine( timeElapsed );
        PerfEvent?.Invoke( this, new PerfEventArgs() );
    }

    public class PerfEventArgs : EventArgs
    {
        public TimeSpan TimeElapsed { get; private set; }
        public PerfEventType EventType { get; private set; }
    }

    public enum PerfEventType
    {
        OnParametersSet,
        OnAfterRender
    }
}