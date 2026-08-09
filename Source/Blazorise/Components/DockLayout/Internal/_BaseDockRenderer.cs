#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Base component for renderers that react to targeted dock layout changes.
/// </summary>
public abstract class _BaseDockRenderer : BaseComponent, IDisposable
{
    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if ( Context is not null )
            Context.Changed += OnContextChanged;
    }

    private void OnContextChanged( DockLayoutChange change )
    {
        if ( !IsAffected( change ) )
            return;

        OnDockLayoutChanged( change );
        StateHasChanged();
    }

    /// <summary>
    /// Determines whether the component is affected by a dock layout change.
    /// </summary>
    private protected abstract bool IsAffected( DockLayoutChange change );

    /// <summary>
    /// Updates component state before rendering a dock layout change.
    /// </summary>
    private protected abstract void OnDockLayoutChanged( DockLayoutChange change );

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing && Context is not null )
            Context.Changed -= OnContextChanged;

        base.Dispose( disposing );
    }

    #endregion

    #region Properties

    [CascadingParameter] internal DockLayoutContext Context { get; set; }

    #endregion
}