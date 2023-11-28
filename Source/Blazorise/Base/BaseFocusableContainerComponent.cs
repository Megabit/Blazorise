#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Base component that is used as a container for focusable components.
/// </summary>
public abstract class BaseFocusableContainerComponent : BaseComponent, IFocusableContainerComponent, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// A focusable components placed inside of a modal.
    /// </summary>
    /// <remarks>
    /// Only one component can be focused, but the reason why we hold the list
    /// of components is in case we change Autofocus="true" from one component to the other.
    /// And because order of rendering is important, we must make sure that the last component
    /// does NOT set focusableComponent to null.
    /// </remarks>
    private List<IFocusableComponent> focusableComponents;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            if ( focusableComponents != null )
            {
                focusableComponents.Clear();
                focusableComponents = null;
            }
        }

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Handles the focus of the nested component.
    /// </summary>
    /// <returns></returns>
    protected Task HandleFocusableComponent()
    {
        // only one component can be focused
        if ( HasFocusableComponents )
        {
            var firstFocusableComponent = FocusableComponents.FirstOrDefault( x => x.Autofocus );

            // take first component if Autofocus is unspecified
            if ( firstFocusableComponent is null )
                firstFocusableComponent = FocusableComponents.FirstOrDefault();

            if ( firstFocusableComponent != null )
            {
                return firstFocusableComponent.Focus();
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void NotifyFocusableComponentInitialized( IFocusableComponent focusableComponent )
    {
        if ( focusableComponent is null )
            return;

        if ( !FocusableComponents.Contains( focusableComponent ) )
        {
            FocusableComponents.Add( focusableComponent );
        }
    }

    /// <inheritdoc/>
    public void NotifyFocusableComponentRemoved( IFocusableComponent focusableComponent )
    {
        if ( focusableComponent is null )
            return;

        if ( FocusableComponents.Contains( focusableComponent ) )
        {
            FocusableComponents.Remove( focusableComponent );
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the list of focusable components.
    /// </summary>
    protected IList<IFocusableComponent> FocusableComponents
        => focusableComponents ??= new();

    /// <summary>
    /// Returns true if modal contains and component that should be autofocused.
    /// </summary>
    public bool HasFocusableComponents
        => FocusableComponents.Count > 0;

    /// <summary>
    /// Parent focusable container.
    /// </summary>
    [CascadingParameter] protected IFocusableContainerComponent ParentFocusableContainer { get; set; }

    #endregion
}