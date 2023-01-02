#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A wrapper for collapse content.
/// </summary>
public partial class CollapseBody : BaseComponent
{
    #region Members

    private bool visible;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="CollapseBody"/> constructor.
    /// </summary>
    public CollapseBody()
    {
        ContentClassBuilder = new( BuildBodyClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.CollapseBody( ParentCollapse?.InsideAccordion == true ) );
        builder.Append( ClassProvider.CollapseBodyActive( ParentCollapse?.InsideAccordion == true, Visible ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds the classnames for a collapse body element.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    private void BuildBodyClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.CollapseBodyContent( ParentCollapse?.InsideAccordion == true, ParentCollapse?.FirstInAccordion == true, ParentCollapse?.LastInAccordion == true ) );
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        ContentClassBuilder.Dirty();

        base.DirtyClasses();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Body container class builder.
    /// </summary>
    protected ClassBuilder ContentClassBuilder { get; private set; }

    /// <summary>
    /// Gets body container class-names.
    /// </summary>
    protected string ContentClassNames => ContentClassBuilder.Class;

    /// <summary>
    /// Gets or sets the content visibility.
    /// </summary>
    [CascadingParameter( Name = "CollapseVisible" )]
    public bool Visible
    {
        get => visible;
        set
        {
            visible = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets the cascaded parent collapse component.
    /// </summary>
    [CascadingParameter] public Collapse ParentCollapse { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="CollapseBody"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}