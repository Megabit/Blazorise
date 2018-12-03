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
    public abstract class BaseCustom : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.AddonLabel() );

            base.RegisterClasses();
        }

        protected void HandleTextChange( UIChangeEventArgs e )
        {
            Text = e?.Value?.ToString();
            TextChanged?.Invoke( Text );
        }

        protected RenderFragment CreateDynamicComponent() => builder =>
        {
            builder.OpenComponent( 0, Type );
            builder.AddAttribute( 1, nameof( Text ), Text );
            builder.AddAttribute( 2, nameof( TextChanged ), TextChanged );
            builder.AddAttribute( 3, nameof( ChildContent ), ChildContent );
            builder.CloseComponent();
        };

        #endregion

        #region Properties

        [Parameter] protected RenderFragment ChildContent { get; set; }

        [Parameter] protected string Text { get; set; }

        [Parameter] protected Action<string> TextChanged { get; set; }

        [Parameter] protected Type Type { get; set; }

        #endregion
    }
}
