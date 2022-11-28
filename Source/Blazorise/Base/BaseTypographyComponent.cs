#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Base class for all text-based components.
/// </summary>
public abstract class BaseTypographyComponent : BaseComponent
{
    #region Members       

    private bool italic = false;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TextItalic(), Italic );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Handles the component click event.
    /// </summary>
    /// <returns></returns>
    protected async Task OnClickHandler()
    {
        if ( CopyToClipboard )
            await JSUtilitiesModule.CopyToClipboard( ElementRef, ElementId );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the <see cref="IJSUtilitiesModule"/> instance.
    /// </summary>
    [Inject] public IJSUtilitiesModule JSUtilitiesModule { get; set; }

    /// <summary>
    /// Italicize text if set to true.
    /// </summary>
    [Parameter]
    public bool Italic
    {
        get => italic;
        set
        {
            italic = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// If true, the content of the component will be copied to clipboard on click event.
    /// </summary>
    [Parameter] public bool CopyToClipboard { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this component.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}