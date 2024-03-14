#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise.Docs.Core;

/// <summary>
/// Used to hold a list of disposable that are going to be disposed after
/// some long running process has ended, like sending an email.
/// </summary>
public class DisposableContainer : IDisposable
{
    private readonly List<IDisposable> disposables;

    public DisposableContainer()
    {
        disposables = new List<IDisposable>();
    }

    public void Add( IDisposable disposable )
    {
        disposables.Add( disposable );
    }

    public void Dispose()
    {
        foreach ( var disposable in disposables )
        {
            disposable.Dispose();
        }
    }
}
