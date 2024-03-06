#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A wrapper for accordion content.
/// </summary>
public partial class AccordionBody : BaseComponent
{
    #region Members

    private bool visible;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="AccordionBody"/> constructor.
    /// </summary>
    public AccordionBody()
    {
        ContentClassBuilder = new( BuildBodyClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.AccordionBody() );
        builder.Append( ClassProvider.AccordionBodyActive( Visible ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds the classnames for a accordion body element.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    private void BuildBodyClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.AccordionBodyContent( ParentAccordionItem?.FirstInAccordion == true, ParentAccordionItem?.LastInAccordion == true ) );
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
    [CascadingParameter( Name = "AccordionItemVisible" )]
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
    /// Gets or sets the cascaded parent accordion item component.
    /// </summary>
    [CascadingParameter] public AccordionItem ParentAccordionItem { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="AccordionBody"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}