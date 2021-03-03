#region Using directives
using System;
using System.Collections.ObjectModel;
#endregion

namespace Blazorise.Utilities
{
    public sealed class CompositeDisposable : Collection<IDisposable>, IDisposable
    {
        #region Methods

        public void Dispose()
        {
            foreach ( var disposable in Items )
            {
                disposable.Dispose();
            }

            Clear();
        }

        #endregion
    }

    public static class CompositeDisposableEx
    {
        #region Methods

        public static T DisposeWith<T>( this T disposable, CompositeDisposable cleanup )
            where T : IDisposable
        {
            if ( disposable == null )
                throw new ArgumentNullException( nameof( disposable ) );

            if ( cleanup == null )
                throw new ArgumentNullException( nameof( cleanup ) );

            cleanup.Add( disposable );

            return disposable;
        }

        #endregion
    }
}