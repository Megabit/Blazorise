#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A divider is a thin line that groups content in lists and layouts.
/// </summary>
public partial class Divider : BaseComponent, IDisposable
{
    #region Members       

    /// <summary>
    /// Defines the type and style of the divider.
    /// </summary>
    private DividerType? type;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if ( Theme is not null )
        {
            Theme.Changed += OnThemeChanged;
        }

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            if ( Theme is not null )
            {
                Theme.Changed -= OnThemeChanged;
            }
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Divider() );
        builder.Append( ClassProvider.DividerType( ApplyDividerType ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// An event raised when theme settings changes.
    /// </summary>
    /// <param name="sender">An object that raised the event.</param>
    /// <param name="eventArgs"></param>
    private void OnThemeChanged( object sender, EventArgs eventArgs )
    {
        DirtyClasses();
        DirtyStyles();

        InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the divider type to apply, based on current theme settings.
    /// </summary>
    protected DividerType ApplyDividerType
        => DividerType.GetValueOrDefault( Theme?.DividerOptions?.DividerType ?? Blazorise.DividerType.Solid );

    /// <summary>
    /// Defines the type and style of the divider.
    /// </summary>
    [Parameter]
    public DividerType? DividerType
    {
        get => type;
        set
        {
            type = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Label that will appear between the solid lines when <see cref="DividerType"/> is set as <see cref="DividerType.TextContent"/>.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Cascaded theme settings.
    /// </summary>
    [CascadingParameter] public Theme Theme { get; set; }

    #endregion
}