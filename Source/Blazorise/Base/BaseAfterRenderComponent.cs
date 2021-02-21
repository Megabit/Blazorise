using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Base
{
    public abstract class BaseAfterRenderComponent : ComponentBase, IDisposable
    {
        #region Members

        private bool disposedValue;

        /// <summary>
        /// A stack of functions to execute after the rendering.
        /// </summary>
        protected Queue<Func<Task>> executeAfterRenderQueue;

        /// <summary>
        /// Indicates if component has been rendered in the browser.
        /// </summary>
        protected bool Rendered { get; private set; }

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

        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            Rendered = true;
            if ( executeAfterRenderQueue?.Count > 0 )
            {
                while ( executeAfterRenderQueue.Count > 0 )
                {
                    await executeAfterRenderQueue.Dequeue()();
                }
            }

            await base.OnAfterRenderAsync( firstRender );
        }

        protected virtual void Dispose( bool disposing )
        {
            if ( !disposedValue )
            {
                disposedValue = true;

                if ( disposing )
                {
                    executeAfterRenderQueue?.Clear();
                    executeAfterRenderQueue = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose( disposing: true );
            GC.SuppressFinalize( this );
        }

        #endregion 

    }
}