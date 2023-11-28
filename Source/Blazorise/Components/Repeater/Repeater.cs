#region Using directives
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Repeater component that will render the <see cref="ChildContent"/> for every item in <see cref="Items"/>.
/// Has support for <see cref="INotifyCollectionChanged"/> so it will update the rendered list of items when the collection changes.
/// </summary>
/// <typeparam name="TItem">the type to render</typeparam>
public class Repeater<TItem> : IComponent, IDisposable
{
    #region Members

    private RenderHandle renderHandle;

    private INotifyCollectionChanged collection;

    #endregion

    #region Constructors

    /// <summary>
    /// <see cref="Repeater{TItem}"/> finalizer
    /// </summary>
    ~Repeater()
    {
        Dispose( false );
    }

    #endregion

    #region Methods

    /// <inheritdoc cref="IComponent.Attach"/>
    public virtual void Attach( RenderHandle handle )
    {
        renderHandle = handle;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        Dispose( true );
    }

    /// <summary>
    /// Cleanup unmanaged resources and registered event handlers.
    /// </summary>
    /// <param name="disposing"><value>true</value> when disposing, <value>false</value> when finalizing</param>
    protected virtual void Dispose( bool disposing )
    {
        if ( disposing && collection is not null )
        {
            collection.CollectionChanged -= OnCollectionChanged;
            collection = null;
        }
    }

    /// <inheritdoc cref="IComponent.SetParametersAsync"/>
    public virtual Task SetParametersAsync( ParameterView parameters )
    {
        var current = Items;

        parameters.SetParameterProperties( this );

        if ( ReferenceEquals( current, Items ) )
        {
            RenderItems();
        }
        else
        {
            if ( collection is not null )
            {
                collection.CollectionChanged -= OnCollectionChanged;
                collection = null;
            }

            if ( Items is INotifyCollectionChanged collectionChanged )
            {
                collection = collectionChanged;
                collection.CollectionChanged += OnCollectionChanged;
            }

            return CollectionChangedAsync( new( NotifyCollectionChangedAction.Reset ) );
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Occurs when the items collection changes.
    /// </summary>
    private async void OnCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
    {
        await CollectionChangedAsync( e );
    }

    /// <summary>
    /// Occurs when the items collection changes.
    /// </summary>
    protected virtual async Task CollectionChangedAsync( NotifyCollectionChangedEventArgs e )
    {
        if ( renderHandle.IsInitialized )
        {
            if ( renderHandle.Dispatcher.CheckAccess() )
            {
                RenderItems();
            }
            else
            {
                await renderHandle.Dispatcher.InvokeAsync( RenderItems );
            }
        }

        await CollectionChanged.InvokeAsync( e );
    }

    /// <summary>
    /// Renders the items in the collection.
    /// </summary>
    public virtual void RenderItems()
    {
        renderHandle.Render( builder =>
        {
            if ( Items is null )
            {
                return;
            }

            var skip = Skip ?? 0;
            var take = Take ?? long.MaxValue;

            foreach ( var (item, index) in Items.Select( ( x, i ) => (x, i) ) )
            {
                if ( index < skip )
                {
                    continue;
                }

                if ( index >= take )
                {
                    break;
                }

                builder.AddContent( index, ChildContent, item );
            }
        } );
    }

    #endregion

    #region Properties

    /// <summary>
    /// The items to render. When this is <see cref="INotifyCollectionChanged"/> it will hookup collection change listeners.
    /// </summary>
    [Parameter] public IEnumerable<TItem> Items { get; set; }

    /// <summary>
    /// [Optional] The number of items to take.
    /// </summary>
    [Parameter] public long? Take { get; set; }

    /// <summary>
    /// [Optional] The number of items to skip.
    /// </summary>
    [Parameter] public long? Skip { get; set; }

    /// <summary>
    /// Occurs when <see cref="Items"/> collection changes.
    /// </summary>
    [Parameter] public EventCallback<NotifyCollectionChangedEventArgs> CollectionChanged { get; set; }

    /// <summary>
    /// The content to render per item.
    /// </summary>
    [Parameter] public RenderFragment<TItem> ChildContent { get; set; }

    #endregion
}