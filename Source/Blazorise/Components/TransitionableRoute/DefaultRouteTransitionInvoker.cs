using System.Threading.Tasks;

namespace Blazorise;

public class DefaultRouteTransitionInvoker : IRouteTransitionInvoker
{
    public Task InvokeRouteTransitionAsync( RouteTransition transition )
    {
        return Task.CompletedTask;
    }
}