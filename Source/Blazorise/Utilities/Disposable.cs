#region Using directives
using System;
#endregion

namespace Blazorise.Utilities
{
    public sealed class Disposable : IDisposable
    {
        #region Members

        private Action action;

        #endregion

        #region Constructors

        private Disposable( Action action )
        {
            this.action = action ?? throw new ArgumentNullException( nameof( action ) );
        }

        #endregion

        #region Methods

        public static IDisposable Create( Action action ) => new Disposable( action );

        public void Dispose()
        {
            action?.Invoke();
            action = null;
        }

        #endregion
    }
}