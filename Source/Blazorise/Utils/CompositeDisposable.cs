using System;
using System.Collections.ObjectModel;

namespace Blazorise.Utils
{
    public sealed class CompositeDisposable : Collection<IDisposable>, IDisposable
    {
        public void Dispose()
        {
            foreach ( var disposable in Items )
            {
                disposable.Dispose();
            }

            Clear();
        }
    }

    public static class CompositeDisposableEx
    {
        public static T DisposeWith<T>( this T disposable, CompositeDisposable cleanup )
            where T : IDisposable
        {
            if ( disposable == null ) throw new ArgumentNullException( nameof(disposable) );
            if ( cleanup == null ) throw new ArgumentNullException( nameof(cleanup) );

            cleanup.Add( disposable );

            return disposable;
        }
    }
}