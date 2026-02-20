#region Using directives
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise.RichTextEdit;

/// <summary>
/// Base <see cref="Blazorise.RichTextEdit"/> component
/// </summary>
public abstract class BaseRichTextEditComponent : BaseInputComponent<string>
{
    #region Constructors

    /// <summary>
    /// Creates a new <see cref="BaseRichTextEditComponent"/>
    /// </summary>
    protected BaseRichTextEditComponent() { }

    #endregion

    #region Methods

    /// <summary>
    /// Executes given action after the rendering is done.
    /// </summary>
    /// <remarks>Don't await this on the UI thread, because that will cause a deadlock.</remarks>
    protected async Task<T> ExecuteAfterRender<T>( Func<Task<T>> action, CancellationToken token = default )
    {
        var source = new TaskCompletionSource<T>();

        token.Register( () => source.TrySetCanceled() );

        ExecuteAfterRender( async () =>
        {
            try
            {
                var result = await action();
                source.TrySetResult( result );
            }
            catch ( TaskCanceledException )
            {
                source.TrySetCanceled();
            }
            catch ( Exception e )
            {
                source.TrySetException( e );
            }
        } );

        return await source.Task.ConfigureAwait( false );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    #endregion
}