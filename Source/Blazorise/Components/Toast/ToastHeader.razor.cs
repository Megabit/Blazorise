#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// A Toast subcomponent to show the toast header.
/// </summary>
public partial class ToastHeader : BaseComponent
{
    #region Members

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ToastHeader() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// The content to be rendered inside the ToastHeader.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}