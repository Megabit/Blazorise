#region Using directives
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A modal provider to be set at the root of your app, providing a programmatic way to invoke modals with custom content by using ModalService.
    /// </summary>
    public partial class ModalProvider : ComponentBase
    {
        #region Members

        private Modal modalRef;
        private RenderFragment childContent;
        private bool useModalStructure = true;
        private string title;

        #endregion

        #region Constructors



        #endregion

        #region Methods

        ///inheritdoc
        protected override Task OnInitializedAsync()
        {
            ModalService.SetModalProvider( this );
            return base.OnInitializedAsync();
        }


        internal Task Show( RenderFragment childContent )
        {
            this.childContent = childContent;
            return modalRef.Show();
        }


        internal Task Hide()
            => modalRef.Hide();

        #endregion

        #region Properties

        ///inheritdoc
        [Inject] public IModalService ModalService { get; set; }

        #endregion
    }

}
