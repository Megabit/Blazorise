#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Themes;

/// <summary>
/// Publishes changes to the scoped theme to root-level cascading parameter subscribers.
/// </summary>
internal sealed class ThemeCascadingValueSource : CascadingValueSource<Theme>, IDisposable
{
    #region Members

    private readonly Theme theme;

    #endregion

    #region Constructors

    public ThemeCascadingValueSource( Theme theme )
        : base( theme, isFixed: false )
    {
        this.theme = theme;
        theme.Changed += OnThemeChanged;
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public void Dispose()
    {
        theme.Changed -= OnThemeChanged;
    }

    private void OnThemeChanged( object sender, EventArgs eventArgs )
    {
        _ = NotifyChangedAsync();
    }

    #endregion
}