#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseAfterRenderComponent : ComponentBase, IDisposable
    {
        #region Members

        /// <summary>
        /// A stack of functions to execute after the rendering.
        /// </summary>
        private Queue<Func<Task>> executeAfterRenderQueue;

        #endregion

        #region Methods

        /// <summary>
        /// Pushes an action to the stack to be executed after the rendering is done.
        /// </summary>
        /// <param name="action"></param>
        protected void ExecuteAfterRender( Func<Task> action )
        {
            if ( executeAfterRenderQueue == null )
                executeAfterRenderQueue = new Queue<Func<Task>>();

            executeAfterRenderQueue.Enqueue( action );
        }

        /// <inheritdoc/>
        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            Rendered = true;

            if ( executeAfterRenderQueue?.Count > 0 )
            {
                while ( executeAfterRenderQueue.Count > 0 )
                {
                    var action = executeAfterRenderQueue.Dequeue();

                    await action();
                }
            }

            await base.OnAfterRenderAsync( firstRender );
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="BaseComponent"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True if the component is in the disposing process.</param>
        protected virtual void Dispose( bool disposing )
        {
            if ( !Disposed )
            {
                Disposed = true;

                if ( disposing )
                {
                    executeAfterRenderQueue?.Clear();
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates if the component is already fully disposed.
        /// </summary>
        protected bool Disposed { get; private set; }

        /// <summary>
        /// Indicates if component has been rendered in the browser.
        /// </summary>
        protected bool Rendered { get; private set; }

        #endregion
    }
}