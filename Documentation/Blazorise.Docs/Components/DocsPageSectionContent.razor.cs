#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Docs.Components;

public partial class DocsPageSectionContent
{
    #region Methods
    public Task OnDesktopViewClicked()
    {
        FrameView = "desktop";

        return Task.CompletedTask;
    }

    public Task OnTabletViewClicked()
    {
        FrameView = "tablet";

        return Task.CompletedTask;
    }

    public Task OnMobileViewClicked()
    {
        FrameView = "mobile";

        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    private string FrameView { get; set; } = "desktop";

    private IFluentSizing FrameWidth
    {
        get
        {
            switch ( FrameView )
            {
                case "tablet":
                    return Width.Px( 768 );
                case "mobile":
                    return Width.Px( 425 );
                default:
                    return Width.Is100;
            }
        }
    }

    private IFluentSpacing ContentPadding => Outlined
        ? Padding.Is3
        : Padding.Is3.OnY.Is3.FromEnd;

    private IFluentSizing ContentWidth => FullWidth ? Width.Is100 : null;

    private IFluentBorder ContentBorder => Outlined
        ? HasFrame ? Border.Is1.OnAll : Border.Is1.RoundedTop
        : null;

    private bool HasFrame => !string.IsNullOrWhiteSpace( FrameUrl );

    [Parameter] public bool Outlined { get; set; }

    [Parameter] public bool FullWidth { get; set; }

    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the responsive preview URL. Used for desktop previews too when no child content is supplied.
    /// </summary>
    [Parameter] public string FrameUrl { get; set; }

    /// <summary>
    /// Gets or sets the preview height in pixels.
    /// </summary>
    [Parameter] public int FrameHeight { get; set; } = 500;

    /// <summary>
    /// Gets or sets the accessible title of the preview frame.
    /// </summary>
    [Parameter] public string FrameTitle { get; set; } = "Example preview";

    [Parameter] public string Code { get; set; }

    [Parameter] public string CodeTitle { get; set; } = "Example.razor";

    [Parameter] public IReadOnlyList<DocsCodeSource> AdditionalCodes { get; set; }

    [Parameter] public bool CopyFullExample { get; set; }

    [Parameter] public bool ShowCode { get; set; } = true;

    /// <summary>
    /// Specifies whether the source code block is expanded by default.
    /// </summary>
    [Parameter] public bool Expanded { get; set; }

    #endregion
}