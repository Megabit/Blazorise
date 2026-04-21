using System.Threading.Tasks;
using Bunit;
using Xunit;

namespace Blazorise.Tests;

public class AsyncBunitContext : Bunit.BunitContext, IAsyncLifetime
{
    public Task InitializeAsync()
        => Task.CompletedTask;

    async Task IAsyncLifetime.DisposeAsync()
        => await base.DisposeAsync();

    public new void Dispose()
        => base.DisposeAsync().AsTask().GetAwaiter().GetResult();

    public new ValueTask DisposeAsync()
        => base.DisposeAsync();
}