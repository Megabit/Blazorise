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
    #region Members

    private readonly ClassBuilder titleClassBuilder;

    private readonly ClassBuilder descriptionClassBuilder;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new <see cref="PropertyGridHelp"/> component.
    /// </summary>
    public PropertyGridHelp()
    {
        titleClassBuilder = new( BuildTitleClasses );
        descriptionClassBuilder = new( BuildDescriptionClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.PropertyGridHelp() );

        base.BuildClasses( builder );
    }

    private void BuildTitleClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.PropertyGridHelpTitle() );
    }

    private void BuildDescriptionClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.PropertyGridHelpDescription() );
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        titleClassBuilder.Dirty();
        descriptionClassBuilder.Dirty();

        base.DirtyClasses();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the provider class for the help title.
    /// </summary>
    protected string TitleClassNames => titleClassBuilder.Class;

    /// <summary>
    /// Gets the provider class for the help description.
    /// </summary>
    protected string DescriptionClassNames => descriptionClassBuilder.Class;

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