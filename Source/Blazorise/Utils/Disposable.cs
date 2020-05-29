using System;

namespace Blazorise.Utils
{
    internal sealed class Disposable : IDisposable
    {
        private Action action;

        private Disposable(Action action)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public static IDisposable Create(Action action) => new Disposable(action);

        public void Dispose()
        {
            action?.Invoke();
            action = null;
        }
    }
}