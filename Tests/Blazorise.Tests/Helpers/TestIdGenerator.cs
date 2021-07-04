using System.Threading;

namespace Blazorise.Tests.Helpers
{
    public class TestIdGenerator : IIdGenerator
    {
        int id;

        public string Generate
        {
            get
            {
                return Interlocked.Increment(ref id).ToString();
            }
        }
    }
}