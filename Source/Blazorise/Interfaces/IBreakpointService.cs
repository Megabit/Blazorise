#region Using directives
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise;

/// <summary>
/// Shared responsive breakpoint service for the current Blazor circuit.
/// </summary>
public interface IBreakpointService : IAsyncDisposable
{
    /// <summary>
    /// Raised whenever the current breakpoint changes.
    /// </summary>
    event Func<Breakpoint, Task> Changed;

    /// <summary>
    /// Gets the current breakpoint.
    /// </summary>
    Breakpoint Current { get; }

    /// <summary>
    /// Gets whether the service has resolved the current client breakpoint.
    /// </summary>
    bool IsResolved { get; }

    /// <summary>
    /// Ensures the breakpoint listener has been initialized on the client.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task EnsureInitializedAsync();

    /// <summary>
    /// Determines whether the current breakpoint is at least the requested breakpoint.
    /// </summary>
    /// <param name="breakpoint">Breakpoint to compare against.</param>
    /// <returns>True if the current breakpoint is at least the requested value.</returns>
    bool IsAtLeast( Breakpoint breakpoint );

    /// <summary>
    /// Determines whether the current breakpoint is below the requested breakpoint.
    /// </summary>
    /// <param name="breakpoint">Breakpoint to compare against.</param>
    /// <returns>True if the current breakpoint is below the requested value.</returns>
    bool IsBelow( Breakpoint breakpoint );
}