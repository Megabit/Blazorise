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
    public abstract class BaseSelect : BaseInputComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Select() )
                .If( () => ClassProvider.SelectSize( Size ), () => Size != Size.None );

            base.RegisterClasses();
        }

        protected void SelectionChangedHandler( UIChangeEventArgs e )
        {
            SelectedValueChanged?.Invoke( e.Value?.ToString() );
        }

        #endregion

        #region Properties

        [Parameter] protected bool IsMultiple { get; set; }

        /// <summary>
        /// Occurs when the selected item value has changed.
        /// </summary>
        [Parameter] protected Action<string> SelectedValueChanged { get; set; }

        [CascadingParameter] protected BaseAddons ParentAddons { get; set; }

        #endregion
    }
}
