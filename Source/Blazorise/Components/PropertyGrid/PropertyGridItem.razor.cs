#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Presents a property label and its editor inside a <see cref="PropertyGridGroup"/>.
/// </summary>
public partial class PropertyGridItem : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.PropertyGridItem() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the provider class for the item label.
    /// </summary>
    protected string LabelClassNames => ClassProvider.PropertyGridItemLabel();

    /// <summary>
    /// Gets the provider class for the item body.
    /// </summary>
    protected string BodyClassNames => ClassProvider.PropertyGridItemBody();

    /// <summary>
    /// Indicates whether an action button is rendered after the property editor.
    /// </summary>
    protected bool HasAction => ActionVisible && ( ActionClicked.HasDelegate || ActionContent is not null || ActionTemplate is not null );

    /// <summary>
    /// Defines the property label.
    /// </summary>
    [Parameter] public string Label { get; set; }

    /// <summary>
    /// Defines custom content for the property label. When specified, it takes precedence over <see cref="Label"/>.
    /// </summary>
    [Parameter] public RenderFragment LabelContent { get; set; }

    /// <summary>
    /// Defines the size of the property editor action.
    /// </summary>
    [Parameter] public Size Size { get; set; } = Size.Small;

    /// <summary>
    /// Defines whether the property editor action is visible.
    /// </summary>
    [Parameter] public bool ActionVisible { get; set; } = true;

    /// <summary>
    /// Defines whether the property editor action is disabled.
    /// </summary>
    [Parameter] public bool ActionDisabled { get; set; }

    /// <summary>
    /// Defines the property editor action color.
    /// </summary>
    [Parameter] public Color ActionColor { get; set; } = Color.Light;

    /// <summary>
    /// Defines the property editor action icon.
    /// </summary>
    [Parameter] public object ActionIcon { get; set; }

    /// <summary>
    /// Defines the property editor action text.
    /// </summary>
    [Parameter] public string ActionText { get; set; }

    /// <summary>
    /// Defines the accessible title for the property editor action.
    /// </summary>
    [Parameter] public string ActionTitle { get; set; }

    /// <summary>
    /// Defines custom content for the property editor action.
    /// </summary>
    [Parameter] public RenderFragment ActionContent { get; set; }

    /// <summary>
    /// Defines a complete template for the trailing property action.
    /// </summary>
    [Parameter] public RenderFragment ActionTemplate { get; set; }

    /// <summary>
    /// Occurs when the property editor action is clicked.
    /// </summary>
    [Parameter] public EventCallback ActionClicked { get; set; }

    /// <summary>
    /// Specifies the property editor content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}