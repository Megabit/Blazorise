#region Using directives
using Microsoft.AspNetCore.Components;
using System.Text;
#endregion

namespace Blazorise.Docs.Components
{
    public partial class DocsPageSectionContent
    {
        #region Methods

        #endregion

        #region Properties

        private string ClassNames
        {
            get
            {
                var sb = new StringBuilder( "docs-page-section-content" );

                if ( Outlined )
                    sb.Append( " docs-page-section-content-outlined" );

                if ( FullWidth )
                    sb.Append( " docs-page-section-content-fullwidth" );

                return sb.ToString();
            }
        }

        [Parameter] public bool Outlined { get; set; }

        [Parameter] public bool FullWidth { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
