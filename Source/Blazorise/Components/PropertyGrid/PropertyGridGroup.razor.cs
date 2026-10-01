#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Groups related items inside a <see cref="PropertyGrid"/>.
/// </summary>
public partial class PropertyGridGroup : BaseComponent
{
    #region Members

    private readonly ClassBuilder headerClassBuilder;

    private readonly ClassBuilder toggleClassBuilder;

    private readonly ClassBuilder bodyClassBuilder;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new <see cref="PropertyGridGroup"/> component.
    /// </summary>
    public PropertyGridGroup()
    {
        headerClassBuilder = new( BuildHeaderClasses );
        toggleClassBuilder = new( BuildToggleClasses );
        bodyClassBuilder = new( BuildBodyClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.PropertyGridGroup() );

        base.BuildClasses( builder );
    }

    private void BuildHeaderClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.PropertyGridGroupHeader() );
    }

    private void BuildToggleClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.PropertyGridGroupToggle() );
    }

    private void BuildBodyClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.PropertyGridGroupBody() );
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        headerClassBuilder.Dirty();
        toggleClassBuilder.Dirty();
        bodyClassBuilder.Dirty();

        base.DirtyClasses();
    }

    private async Task OnToggleExpandedHandler()
    {
        if ( !Expandable )
        {
            return;
        }

        Expanded = !Expanded;

        await ExpandedChanged.InvokeAsync( Expanded );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the provider class for the group header.
    /// </summary>
    protected string HeaderClassNames => headerClassBuilder.Class;

    /// <summary>
    /// Gets the provider class for the group header toggle.
    /// </summary>
    protected string ToggleClassNames => toggleClassBuilder.Class;

    /// <summary>
    /// Gets the provider class for the group body.
    /// </summary>
    protected string BodyClassNames => bodyClassBuilder.Class;

    /// <summary>
    /// Defines the group title.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Defines custom content for the group header. When specified, it takes precedence over <see cref="Title"/>.
    /// </summary>
    [Parameter] public RenderFragment Header { get; set; }

    /// <summary>
    /// Defines whether the group can be expanded and collapsed.
    /// </summary>
    [Parameter] public bool Expandable { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the group is expanded.
    /// </summary>
    [Parameter] public bool Expanded { get; set; } = true;

    /// <summary>
    /// Occurs after the group expansion state changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ExpandedChanged { get; set; }

    /// <summary>
    /// Defines the icon shown while the group is expanded.
    /// </summary>
    [Parameter] public object ExpandedIcon { get; set; } = IconName.ChevronDown;

    /// <summary>
    /// Defines the icon shown while the group is collapsed.
    /// </summary>
    [Parameter] public object CollapsedIcon { get; set; } = IconName.ChevronRight;

    /// <summary>
    /// Defines the accessible title used to expand the group.
    /// </summary>
    [Parameter] public string ExpandTitle { get; set; } = "Expand group";

    /// <summary>
    /// Defines the accessible title used to collapse the group.
    /// </summary>
    [Parameter] public string CollapseTitle { get; set; } = "Collapse group";

    /// <summary>
    /// Specifies the property items to be rendered inside this <see cref="PropertyGridGroup"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}