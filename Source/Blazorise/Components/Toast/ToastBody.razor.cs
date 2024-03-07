#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// A Toast subcomponent to show the toast content.
/// </summary>
public partial class ToastBody : BaseComponent
{
    #region Members

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ToastBody() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// The content to be rendered inside the ToastBody.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}