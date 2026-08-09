#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Displays help for the selected property.
/// </summary>
public partial class PropertyGridHelp : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.PropertyGridHelp() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the provider class for the help title.
    /// </summary>
    protected string TitleClassNames => ClassProvider.PropertyGridHelpTitle();

    /// <summary>
    /// Gets the provider class for the help description.
    /// </summary>
    protected string DescriptionClassNames => ClassProvider.PropertyGridHelpDescription();

    /// <summary>
    /// Defines the help title.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Defines the help description.
    /// </summary>
    [Parameter] public string Description { get; set; }

    /// <summary>
    /// Defines custom help content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}