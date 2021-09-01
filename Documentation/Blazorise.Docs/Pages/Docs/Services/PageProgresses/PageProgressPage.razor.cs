using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Docs.Pages.Docs.Services.PageProgresses
{
    public partial class PageProgressPage : IDisposable
    {
        #region Methods

        public void Dispose()
        {
            if ( PageProgressService != null )
            {
                // setting it to -1 will hide the progress bar
                PageProgressService.Go( -1 );
            }
        }

        #endregion

        #region Properties

        [Inject] private IPageProgressService PageProgressService { get; set; }

        #endregion
    }
}
