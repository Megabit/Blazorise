#region Using directives
using System;
using System.Collections.ObjectModel;
#endregion

namespace Blazorise.Utilities;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
        if ( disposable is null )
            throw new ArgumentNullException( nameof( disposable ) );

        if ( cleanup is null )
            throw new ArgumentNullException( nameof( cleanup ) );

        cleanup.Add( disposable );

        return disposable;
    }

    #endregion
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member