#region Using directives
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
        ? ShowFrame ? Border.Is1.OnAll : Border.Is1.RoundedTop
        : null;

    bool ShowFrame => !string.IsNullOrEmpty( FrameUrl );

    [Parameter] public bool Outlined { get; set; }

    [Parameter] public bool FullWidth { get; set; }

    [Parameter] public RenderFragment ChildContent { get; set; }

    [Parameter] public string FrameUrl { get; set; }

    [Parameter] public string Code { get; set; }

    [Parameter] public bool ShowCode { get; set; } = true;

    #endregion
}