using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorise;

public interface IRouteTransitionInvoker
{
    Task InvokeRouteTransitionAsync( RouteTransition transition );
}