#region Using directives
using System.Text;
using System.Threading.Tasks;
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

    protected string FrameStyle
    {
        get
        {
            var sb = new StringBuilder( "margin-left: auto; margin-right: auto; display: block; vertical-align: middle; height: 100%;" );

            sb.Append( FrameWidth );

            return sb.ToString();
        }
    }
    private string FrameView { get; set; } = "desktop";

    private string FrameWidth
    {
        get
        {
            switch ( FrameView )
            {
                case "tablet":
                    return "width: 768px;";
                case "mobile":
                    return "width: 425px;";
                default:
                    return "width: 100%;";
            }
        }
    }

    private string ClassNames
    {
        get
        {
            var sb = new StringBuilder( "b-docs-page-section-content" );

            if ( Outlined )
                sb.Append( " b-docs-page-section-content-outlined" );

            if ( FullWidth )
                sb.Append( " b-docs-page-section-content-fullwidth" );

            return sb.ToString();
        }
    }

    bool ShowFrame => !string.IsNullOrEmpty( FrameUrl );

    [Parameter] public bool Outlined { get; set; }

    [Parameter] public bool FullWidth { get; set; }

    [Parameter] public RenderFragment ChildContent { get; set; }

    [Parameter] public string FrameUrl { get; set; }

    [Parameter] public string SourceCode { get; set; }

    #endregion
}