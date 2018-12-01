#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseSnackbar : BaseComponent
    {
        #region Members

        private bool isOpen;

        private bool isMultiline;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Snackbar() )
                .If( () => ClassProvider.SnackbarShow(), () => IsOpen )
                .If( () => ClassProvider.SnackbarMultiline(), () => IsMultiline );

            base.RegisterClasses();
        }

        public void Show()
        {
            IsOpen = true;
            StateHasChanged();
        }

        public void Hide()
        {
            IsOpen = false;
            StateHasChanged();
        }

        #endregion

        #region Properties

        [Parameter]
        private bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        private bool IsMultiline
        {
            get => isMultiline;
            set
            {
                isMultiline = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
