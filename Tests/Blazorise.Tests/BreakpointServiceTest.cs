#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.JSInterop;
using Moq;
using Xunit;
#endregion

namespace Blazorise.Tests;

public class BreakpointServiceTest
{
    [Fact]
    public async Task EnsureInitializedAsync_ShouldResolveCurrentBreakpoint()
    {
        Mock<IJSBreakpointModule> breakpointModuleMock = new();

        breakpointModuleMock
            .Setup( x => x.RegisterBreakpoint( It.IsAny<DotNetObjectReference<BreakpointActivatorAdapter>>(), It.IsAny<string>() ) )
            .Returns( ValueTask.CompletedTask );

        breakpointModuleMock
            .Setup( x => x.GetBreakpoint() )
            .Returns( new ValueTask<string>( "tablet" ) );

        BreakpointService breakpointService = new( new IdGenerator(), breakpointModuleMock.Object );

        await breakpointService.EnsureInitializedAsync();

        Assert.True( breakpointService.IsResolved );
        Assert.Equal( Breakpoint.Tablet, breakpointService.Current );
        Assert.True( breakpointService.IsAtLeast( Breakpoint.Small ) );
        Assert.False( breakpointService.IsBelow( Breakpoint.Small ) );
    }

    [Fact]
    public async Task OnBreakpoint_ShouldUpdateCurrentBreakpoint_AndNotifySubscribers()
    {
        Mock<IJSBreakpointModule> breakpointModuleMock = new();

        breakpointModuleMock
            .Setup( x => x.RegisterBreakpoint( It.IsAny<DotNetObjectReference<BreakpointActivatorAdapter>>(), It.IsAny<string>() ) )
            .Returns( ValueTask.CompletedTask );

        breakpointModuleMock
            .SetupSequence( x => x.GetBreakpoint() )
            .Returns( new ValueTask<string>( "tablet" ) )
            .Returns( new ValueTask<string>( "desktop" ) );

        BreakpointService breakpointService = new( new IdGenerator(), breakpointModuleMock.Object );
        Breakpoint changedBreakpoint = Breakpoint.None;

        breakpointService.Changed += ( breakpoint ) =>
        {
            changedBreakpoint = breakpoint;
            return Task.CompletedTask;
        };

        await breakpointService.EnsureInitializedAsync();
        await breakpointService.OnBreakpoint( false );

        Assert.Equal( Breakpoint.Desktop, breakpointService.Current );
        Assert.Equal( Breakpoint.Desktop, changedBreakpoint );
    }
}