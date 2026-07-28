#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Groups related items inside a <see cref="PropertyGrid"/>.
/// </summary>
public partial class PropertyGridGroup : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.PropertyGridGroup() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the provider class for the group header.
    /// </summary>
    protected string HeaderClassNames => ClassProvider.PropertyGridGroupHeader();

    /// <summary>
    /// Gets the provider class for the group body.
    /// </summary>
    protected string BodyClassNames => ClassProvider.PropertyGridGroupBody();

    /// <summary>
    /// Defines the group title.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Defines custom content for the group header. When specified, it takes precedence over <see cref="Title"/>.
    /// </summary>
    [Parameter] public RenderFragment Header { get; set; }

    /// <summary>
    /// Specifies the property items to be rendered inside this <see cref="PropertyGridGroup"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
