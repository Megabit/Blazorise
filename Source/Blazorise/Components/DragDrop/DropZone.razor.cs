#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// Wrapper component that are acting as a drop area for the drop items.
/// </summary>
/// <typeparam name="TItem">Type of the draggable item.</typeparam>
public partial class DropZone<TItem> : BaseComponent<DropZoneClasses, DropZoneStyles>, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// Indicates that the drop operation is allowed.
    /// </summary>
    private bool dropAllowed;

    /// <summary>
    /// Indicates that the dragging operation is in process.
    /// </summary>
    private bool dragging;

    private int dragCounter = 0;

    private Dictionary<TItem, int> indices = new();

    private bool recalculateItems = true;

    private bool shouldRerender = true;

    private IEnumerable<TItem> items;

    private DotNetObjectReference<DropZone<TItem>> dotNetObjectRef;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="DropZone{TItem}"/> constructor.
    /// </summary>
    public DropZone()
    {
        PlaceholderClassBuilder = new( BuildPlaceholderClasses, builder => builder.Append( Classes?.Placeholder ) );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var animationChanged = parameters.IsParameterChanged( Animated )
            || parameters.IsParameterChanged( AnimationDuration )
            || parameters.IsParameterChanged( AllowReorder )
            || parameters.IsParameterChanged( OnlyZone );

        if ( animationChanged )
        {
            shouldRerender = true;
            DirtyClasses();

            if ( Rendered )
                ExecuteAfterRender( UpdateAnimationOptions );
        }

        await base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if ( ParentContainer is not null )
        {
            ParentContainer.TransactionStarted += OnContainerTransactionStarted;
            ParentContainer.TransactionEnded += OnContainerTransactionEnded;
            ParentContainer.RefreshRequested += OnContainerRefreshRequested;
            ParentContainer.TransactionIndexChanged += OnContainerTransactionIndexChanged;
        }

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            await JSModule.Initialize( ElementRef, ElementId );

            if ( ShouldAnimateReorder )
                await UpdateAnimationOptions();
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-drop-zone" );

        if ( ParentContainer?.TransactionInProgress == true && TransactionSourceZoneName != Name && dropAllowed && ( dragCounter > 0 || GetApplyDropClassesOnDragStarted() ) )
            builder.Append( DropAllowedClass ?? ParentContainer?.DropAllowedClass ?? "b-drop-zone-drop-allowed" );

        if ( ParentContainer?.TransactionInProgress == true && TransactionSourceZoneName != Name && !dropAllowed && ( dragCounter > 0 || GetApplyDropClassesOnDragStarted() ) )
            builder.Append( DropNotAllowedClass ?? ParentContainer?.DropNotAllowedClass ?? "b-drop-zone-drop-not-allowed" );

        if ( dragging )
            builder.Append( GetDraggingClass() );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds the classnames for a placeholder.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    private void BuildPlaceholderClasses( ClassBuilder builder )
    {
        builder.Append( "draggable-placeholder" );

        if ( AllowReorder == false || ( ParentContainer?.TransactionInProgress == false || ParentContainer.TransactionCurrentZoneName != Name ) )
            builder.Append( ClassProvider.Display( DisplayType.None, new DisplayDefinition() ) );
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        PlaceholderClassBuilder.Dirty();

        base.DirtyClasses();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            if ( Rendered )
            {
                await JSModule.SafeDestroy( ElementRef, ElementId );
            }

            if ( ParentContainer is not null )
            {
                ParentContainer.TransactionStarted -= OnContainerTransactionStarted;
                ParentContainer.TransactionEnded -= OnContainerTransactionEnded;
                ParentContainer.RefreshRequested -= OnContainerRefreshRequested;
                ParentContainer.TransactionIndexChanged -= OnContainerTransactionIndexChanged;
            }

            DisposeDotNetObjectRef( dotNetObjectRef );
            dotNetObjectRef = null;
        }

        await base.DisposeAsync( disposing );
    }

    private void OnContainerTransactionStarted( object sender, DraggableTransaction<TItem> e )
    {
        shouldRerender = true;
        DirtyClasses();

        if ( GetApplyDropClassesOnDragStarted() )
        {
            var dropResult = ItemCanBeDropped();

            dropAllowed = dropResult.Item2;
        }

        InvokeAsync( StateHasChanged );
    }

    private void OnContainerTransactionEnded( object sender, DraggableTransactionEnded<TItem> e )
    {
        dragCounter = 0;

        if ( GetApplyDropClassesOnDragStarted() )
        {
            dropAllowed = false;
        }

        if ( e.Success )
        {
            if ( e.OriginDropZoneName == Name && e.DestinationDropZoneName != e.OriginDropZoneName )
            {
                indices.Remove( e.Item );
            }

            if ( e.OriginDropZoneName == Name || e.DestinationDropZoneName == Name )
            {
                int index = 0;

                foreach ( var item in indices.OrderBy( x => x.Value ).ToArray() )
                {
                    indices[item.Key] = index++;
                }

                if ( AllowReorder && Reordered is not null )
                {
                    InvokeAsync( () => Reordered.Invoke( new DropZoneOrder<TItem>( e.OriginDropZoneName, e.DestinationDropZoneName, indices ) ) );
                }
            }
        }

        recalculateItems = true;
        shouldRerender = true;

        DirtyClasses();

        InvokeAsync( StateHasChanged );
    }

    private void OnContainerRefreshRequested( object sender, EventArgs e )
    {
        indices.Clear();

        recalculateItems = true;
        shouldRerender = true;

        DirtyClasses();

        InvokeAsync( StateHasChanged );
    }

    private void OnContainerTransactionIndexChanged( object sender, DraggableIndexChangedEventArgs e )
    {
        if ( e.ZoneName != Name && e.OldZoneName != Name )
            return;

        recalculateItems = true;
        shouldRerender = true;

        DirtyClasses();

        InvokeAsync( StateHasChanged );
    }

    private void OnDragEnterHandler()
    {
        dragCounter++;

        var (context, canBeDropped) = ItemCanBeDropped();

        if ( context is null )
        {
            return;
        }

        dropAllowed = canBeDropped;

        ParentContainer.UpdateTransactionZone( Name );

        DirtyClasses();
    }

    private void OnDragLeaveHandler()
    {
        dragCounter--;

        var (context, _) = ItemCanBeDropped();

        if ( context is null )
        {
            return;
        }

        DirtyClasses();
    }

    /// <summary>
    /// Updates the insertion point using the unanimated item layout. Intended for JavaScript interop.
    /// </summary>
    /// <param name="index">Index of the item under the pointer, or -1 for the start of the zone.</param>
    [JSInvokable]
    public Task OnReorderDragOver( int index )
    {
        if ( ShouldAnimateReorder && ParentContainer?.TransactionInProgress == true
            && index >= -1 && index < GetItems().Count() )
        {
            ParentContainer.UpdateTransactionZone( Name );
            ParentContainer.UpdateTransactionIndex( index );
        }

        return Task.CompletedTask;
    }

    private async Task OnDropHandler()
    {
        var (context, canBeDropped) = ItemCanBeDropped();

        if ( context is null )
        {
            return;
        }

        dragCounter = 0;

        if ( !canBeDropped )
        {
            DirtyClasses();

            await ParentContainer.CancelTransaction();

            return;
        }

        if ( AllowReorder )
        {
            if ( ParentContainer.HasTransactionIndexChanged )
            {
                var newIndex = ParentContainer.GetTransactionIndex() + 1;

                if ( ParentContainer.IsTransactionOriginatedFromInside( this.Name ) )
                {
                    var oldIndex = indices[context];

                    if ( ParentContainer.IsItemMovedDownwards )
                    {
                        newIndex -= 1;

                        foreach ( var item in indices.Where( x => x.Value >= oldIndex + 1 && x.Value <= newIndex ).ToArray() )
                        {
                            indices[item.Key] -= 1;
                        }
                    }
                    else
                    {
                        foreach ( var item in indices.Where( x => x.Value >= newIndex && x.Value < oldIndex ).ToArray() )
                        {
                            indices[item.Key] += 1;
                        }
                    }

                    indices[context] = newIndex;
                }
                else
                {
                    foreach ( var item in indices.Where( x => x.Value >= newIndex ).ToArray() )
                    {
                        indices[item.Key] = item.Value + 1;
                    }

                    indices.Add( context, newIndex );
                }
            }
        }
        else
        {
            indices.Clear();
        }

        await ParentContainer.CommitTransaction( Name, AllowReorder );

        DirtyClasses();
    }

    private (TItem, bool) ItemCanBeDropped()
    {
        if ( ParentContainer is null || ParentContainer.TransactionInProgress == false )
            return (default( TItem ), false);

        var item = ParentContainer.GetTransactionItem();

        if ( item is null )
            return (default( TItem ), false);

        var canBeDropped = true;

        if ( DropAllowed is not null )
        {
            canBeDropped = DropAllowed( item );
        }
        else if ( ParentContainer.DropAllowed is not null )
        {
            canBeDropped = ParentContainer.DropAllowed( item, Name );
        }

        return (item, canBeDropped);
    }

    private Task OnDragStarted()
    {
        dragging = true;

        return Task.CompletedTask;
    }

    private Task OnDragEnded( TItem item )
    {
        dragging = false;

        return Task.CompletedTask;
    }

    private IEnumerable<TItem> GetItems()
    {
        if ( recalculateItems )
        {
            recalculateItems = false;

            Func<TItem, bool> predicate = ( item ) => ParentContainer.ItemsFilter( item, Name ?? string.Empty );

            if ( ItemsFilter is not null )
            {
                predicate = ItemsFilter;
            }

            items = ( ParentContainer?.Items ?? Enumerable.Empty<TItem>() ).Where( predicate ).OrderBy( x => GetItemIndex( x ) ).ToArray();
        }

        return items ?? Enumerable.Empty<TItem>();
    }

    private bool GetItemDisabled( TItem item )
    {
        var disabled = false;

        var predicate = ItemDisabled ?? ParentContainer?.ItemDisabled;

        if ( predicate is not null )
        {
            disabled = predicate( item );
        }

        return disabled;
    }

    private string GetDraggingClass()
    {
        return DraggingClass ?? ParentContainer?.DraggingClass;
    }

    private string GetItemDisabledClass()
    {
        return DisabledClass ?? ParentContainer?.DisabledClass;
    }

    private string GetItemDraggingClass()
    {
        return ItemDraggingClass ?? ParentContainer?.ItemDraggingClass;
    }

    private bool GetApplyDropClassesOnDragStarted() => ( ApplyDropClassesOnDragStarted ?? ParentContainer?.ApplyDropClassesOnDragStarted ) ?? false;

    private int GetItemIndex( TItem item )
    {
        if ( !indices.ContainsKey( item ) )
            indices.Add( item, indices.Count );

        return indices[item];
    }

    private bool IsOrigin( int index ) => ParentContainer.IsOrigin( index, Name );

    private async Task UpdateAnimationOptions()
    {
        if ( ShouldAnimateReorder )
            dotNetObjectRef ??= CreateDotNetObjectRef( this );

        await JSModule.UpdateOptions( ElementRef, ElementId, dotNetObjectRef, new()
        {
            Animated = ShouldAnimateReorder,
            AnimationDuration = EffectiveAnimationDuration,
        } );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldRender()
    {
        if ( shouldRerender )
        {
            shouldRerender = false;
            return true;
        }

        return shouldRerender;
    }

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets the name of the dropzone that started the transaction.
    /// </summary>
    protected string TransactionSourceZoneName => ParentContainer?.TransactionSourceZoneName;

    /// <summary>
    /// Gets the placeholder template defined for this drop zone or its parent container.
    /// </summary>
    protected RenderFragment<TItem> EffectivePlaceholderTemplate => PlaceholderTemplate ?? ParentContainer?.PlaceholderTemplate;

    /// <summary>
    /// Gets the nonnegative animation duration, in milliseconds.
    /// </summary>
    protected int EffectiveAnimationDuration => Math.Max( 0, AnimationDuration );

    /// <summary>
    /// Indicates whether this zone should animate item movement during reordering.
    /// </summary>
    internal bool ShouldAnimateReorder => Animated && AllowReorder && !OnlyZone && EffectiveAnimationDuration > 0;

    /// <summary>
    /// Gets the transaction activity serialized for JavaScript.
    /// </summary>
    protected string TransactionActiveString => ParentContainer?.TransactionInProgress == true ? "true" : "false";

    /// <summary>
    /// Gets whether this is the current transaction zone serialized for JavaScript.
    /// </summary>
    protected string TransactionCurrentString => ParentContainer?.TransactionInProgress == true && ParentContainer.TransactionCurrentZoneName == Name ? "true" : "false";

    /// <summary>
    /// Placeholder class builder.
    /// </summary>
    protected ClassBuilder PlaceholderClassBuilder { get; private set; }

    /// <summary>
    /// Specifies the <see cref="IJSDragDropModule"/> instance.
    /// </summary>
    [Inject] public IJSDragDropModule JSModule { get; set; }

    /// <summary>
    /// Specifies the unique name of the dropzone.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// The method used to determine if the item belongs to the dropzone.
    /// </summary>
    [Parameter] public Func<TItem, bool> ItemsFilter { get; set; }

    /// <summary>
    /// The render method that is used to render the items withing the dropzone.
    /// </summary>
    [Parameter] public RenderFragment<TItem> ItemTemplate { get; set; }

    /// <summary>
    /// The template used to render the placeholder for the item being reordered. The template receives the item currently being dragged and overrides the template defined by the parent container.
    /// </summary>
    [Parameter] public RenderFragment<TItem> PlaceholderTemplate { get; set; }

    /// <summary>
    /// Determines if the item is allowed to be dropped to this zone.
    /// </summary>
    [Parameter] public Func<TItem, bool> DropAllowed { get; set; }

    /// <summary>
    /// Classname that is applied if dropping to the current zone is allowed.
    /// </summary>
    [Parameter] public string DropAllowedClass { get; set; }

    /// <summary>
    /// Classname that is applied if dropping to the current zone is not allowed.
    /// </summary>
    [Parameter] public string DropNotAllowedClass { get; set; }

    /// <summary>
    /// When true, <see cref="DropAllowedClass"/> or <see cref="DropNotAllowedClass"/> drop classes are applied as soon as a transaction has started.
    /// </summary>
    [Parameter] public bool? ApplyDropClassesOnDragStarted { get; set; }

    /// <summary>
    /// Determines if the item is disabled for dragging and dropping.
    /// </summary>
    [Parameter] public Func<TItem, bool> ItemDisabled { get; set; }

    /// <summary>
    /// Classname that is applied to the dropzone if the result of <see cref="ItemDisabled"/> is false.
    /// </summary>
    [Parameter] public string DisabledClass { get; set; }

    /// <summary>
    /// Classname that is applied to the dropzone when the drag operation has started.
    /// </summary>
    [Parameter] public string DraggingClass { get; set; }

    /// <summary>
    /// Classname that is applied to the drag item when it is being dragged.
    /// </summary>
    [Parameter] public string ItemDraggingClass { get; set; }

    /// <summary>
    /// If true, the reordering of the items will be enabled.
    /// </summary>
    [Parameter] public bool AllowReorder { get; set; }

    /// <summary>
    /// If true, animates item movement during reordering. Requires <see cref="AllowReorder"/>.
    /// Respects the user's reduced motion preference.
    /// </summary>
    [Parameter] public bool Animated { get; set; }

    /// <summary>
    /// Gets or sets the reorder animation duration, in milliseconds.
    /// Values less than or equal to zero disable the animation.
    /// </summary>
    [Parameter] public int AnimationDuration { get; set; } = 200;

    /// <summary>
    /// The callback that is raised when the order of items changed. Only if <see cref="AllowReorder"/> is enabled.
    /// </summary>
    [Parameter] public Func<DropZoneOrder<TItem>, Task> Reordered { get; set; }

    /// <summary>
    /// If true, will only act as a dropable zone and not render any items.
    /// </summary>
    [Parameter] public bool OnlyZone { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="DropZone{TItem}"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Provides the reference to the parent <see cref="DropContainer{TItem}"/> component.
    /// </summary>
    [CascadingParameter] protected DropContainer<TItem> ParentContainer { get; set; }

    #endregion
}