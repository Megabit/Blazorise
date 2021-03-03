#region Using directives
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Utilities
{
    public sealed class AsyncDisposable : IAsyncDisposable
    {
        #region Members

        private Func<ValueTask> action;

        #endregion

        #region Constructors

        private AsyncDisposable( Func<ValueTask> action )
        {
            this.action = action ?? throw new ArgumentNullException( nameof( action ) );
        }

        #endregion

        #region Methods

        public static IAsyncDisposable Create( Func<ValueTask> action )
            => new AsyncDisposable( action );

        public ValueTask DisposeAsync()
        {
            if ( action != null )
            {
                return action.Invoke();
            }

            return ValueTask.CompletedTask;
        }

        #endregion
    }
}