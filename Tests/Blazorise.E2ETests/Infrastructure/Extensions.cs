using System;
using System.Threading.Tasks;

namespace Blazorise.E2ETests.Infrastructure
{
    public static class Extensions
    {
        public static async Task TimeoutAfter( this Task task, TimeSpan timeSpan )
        {
            if ( task == await Task.WhenAny( task, Task.Delay( timeSpan ) ) )
                await task;
            else
                throw new TimeoutException();
        }
    }
}
