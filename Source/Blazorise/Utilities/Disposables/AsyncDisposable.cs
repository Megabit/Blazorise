#region Using directives
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Helper class for asynchronously disposing of short lived objects.
/// </summary>
public sealed class AsyncDisposable : IAsyncDisposable
{
    #region Members

    private readonly Func<ValueTask> action;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="AsyncDisposable"/> constructor.
    /// </summary>
    /// <param name="action">Asynchronous action with operations to run before disposed.</param>
    private AsyncDisposable( Func<ValueTask> action )
    {
        this.action = action ?? throw new ArgumentNullException( nameof( action ) );
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create a new <see cref="AsyncDisposable"/> object.
    /// </summary>
    /// <param name="action">Asynchronous action with operations to run before disposed.</param>
    /// <returns>Returns a new <see cref="IAsyncDisposable"/> object.</returns>
    public static IAsyncDisposable Create( Func<ValueTask> action )
        => new AsyncDisposable( action );

    /// <summary>
    /// Asynchronously disposes of any resources.
    /// </summary>
    public ValueTask DisposeAsync()
    {
        if ( action is not null )
        {
            return action.Invoke();
        }

        return ValueTask.CompletedTask;
    }

    #endregion
}