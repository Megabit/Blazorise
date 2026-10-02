#region Using directives
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Presents a property label and its editor inside a <see cref="PropertyGridGroup"/>.
/// </summary>
public partial class PropertyGridItem : BaseComponent
{
    #region Members

    private readonly ClassBuilder labelClassBuilder;

    private readonly ClassBuilder bodyClassBuilder;

    private readonly ClassBuilder selectedClassBuilder;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new <see cref="PropertyGridItem"/> component.
    /// </summary>
    public PropertyGridItem()
    {
        labelClassBuilder = new( BuildLabelClasses );
        bodyClassBuilder = new( BuildBodyClasses );
        selectedClassBuilder = new( BuildSelectedClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.IsParameterChanged( Selected ) )
        {
            DirtyClasses();
        }

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.PropertyGridItem() );

        base.BuildClasses( builder );
    }

    private void BuildLabelClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.PropertyGridItemLabel() );
    }

    private void BuildBodyClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.PropertyGridItemBody() );
    }

    private void BuildSelectedClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.PropertyGridItemSelected( Selected ) );
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        labelClassBuilder.Dirty();
        bodyClassBuilder.Dirty();
        selectedClassBuilder.Dirty();

        base.DirtyClasses();
    }

    private async Task OnSelectHandler()
    {
        if ( !Selectable || Selected )
        {
            return;
        }

        Selected = true;
        DirtyClasses();

        await SelectedChanged.InvokeAsync( true );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the provider class for the item label.
    /// </summary>
    protected string LabelClassNames => labelClassBuilder.Class;

    /// <summary>
    /// Gets the provider class for the item body.
    /// </summary>
    protected string BodyClassNames => bodyClassBuilder.Class;

    /// <summary>
    /// Gets the provider class for the selected item state.
    /// </summary>
    protected string SelectedClassNames => selectedClassBuilder.Class;

    /// <summary>
    /// Indicates whether trailing content is rendered after the property editor.
    /// </summary>
    protected bool HasAction => ActionContent is not null;

    /// <summary>
    /// Gets the row tab index when selection is enabled.
    /// </summary>
    protected int? TabIndexValue => Selectable ? 0 : null;

    /// <summary>
    /// Gets the accessible selection state.
    /// </summary>
    protected bool? AriaSelected => Selectable ? Selected : null;

    /// <summary>
    /// Defines the property label.
    /// </summary>
    [Parameter] public string Label { get; set; }

    /// <summary>
    /// Defines custom content for the property label. When specified, it takes precedence over <see cref="Label"/>.
    /// </summary>
    [Parameter] public RenderFragment LabelContent { get; set; }

    /// <summary>
    /// Defines whether the property can be selected.
    /// </summary>
    [Parameter] public bool Selectable { get; set; }

    /// <summary>
    /// Defines whether the property is selected.
    /// </summary>
    [Parameter] public bool Selected { get; set; }

    /// <summary>
    /// Occurs after the property selection state changes.
    /// </summary>
    [Parameter] public EventCallback<bool> SelectedChanged { get; set; }

    /// <summary>
    /// Identifies the element that describes the property.
    /// </summary>
    [Parameter] public string AriaDescribedBy { get; set; }

    /// <summary>
    /// Defines the property editor size.
    /// </summary>
    [Parameter] public Size Size { get; set; } = Size.Small;

    /// <summary>
    /// Defines complete trailing content for the property editor.
    /// </summary>
    [Parameter] public RenderFragment ActionContent { get; set; }

    /// <summary>
    /// Specifies the property editor content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}