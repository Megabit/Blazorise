#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Component that handles the <see cref="IPageProgressService"/> to show the page progress.
/// </summary>
public partial class PageProgressProvider : BaseComponent, IDisposable
{
    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        PageProgressService.ProgressChanged += OnProgressChanged;
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            if ( PageProgressService is not null )
            {
                PageProgressService.ProgressChanged -= OnProgressChanged;
            }
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    private async void OnProgressChanged( object sender, PageProgressEventArgs eventArgs )
    {
        Percentage = eventArgs.Percentage;
        Visible = eventArgs.Percentage is null || ( eventArgs.Percentage >= 0 && eventArgs.Percentage <= 100 );
        Color = eventArgs.Options.Color;

        await PageProgressRef.SetValueAsync( eventArgs.Percentage );

        await InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the <see cref="PageProgress"/> reference.
    /// </summary>
    protected PageProgress PageProgressRef { get; set; }

    /// <summary>
    /// Gets or sets the page progress percentage, in range 0-100 or null for indeterminate.
    /// </summary>
    protected int? Percentage { get; set; }

    /// <summary>
    /// Gets or sets the visibility of the page alert.
    /// </summary>
    protected bool Visible { get; set; }

    /// <summary>
    /// Gets or sets the page alert color.
    /// </summary>
    protected Color Color { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="IPageProgressService"/> to which this component is responding.
    /// </summary>
    [Inject] protected IPageProgressService PageProgressService { get; set; }

    #endregion
}