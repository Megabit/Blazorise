#region Using directives
using System;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Helper class for disposing of short lived objects.
/// </summary>
public sealed class Disposable : IDisposable
{
    #region Members

    private Action action;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="Disposable"/> constructor.
    /// </summary>
    /// <param name="action">Action with operations to run before disposed.</param>
    private Disposable( Action action )
    {
        this.action = action ?? throw new ArgumentNullException( nameof( action ) );
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create a new <see cref="Disposable"/> object.
    /// </summary>
    /// <param name="action">Action with operations to run before disposed.</param>
    /// <returns>Returns a new <see cref="IDisposable"/> object.</returns>
    public static IDisposable Create( Action action )
        => new Disposable( action );

    /// <summary>
    /// Disposes of any resources.
    /// </summary>
    public void Dispose()
    {
        action?.Invoke();
        action = null;
    }

    #endregion
}