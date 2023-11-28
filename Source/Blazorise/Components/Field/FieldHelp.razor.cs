#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Sets the field help-text positioned bellow the field.
/// </summary>
public partial class FieldHelp : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.FieldHelp() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// True if the parent <see cref="Field"/> is in horizontal mode.
    /// </summary>
    protected virtual bool ParentIsFieldBody => ParentFieldBody is not null;

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="FieldHelp"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the reference to the parent <see cref="FieldBody"/> component.
    /// </summary>
    [CascadingParameter] protected FieldBody ParentFieldBody { get; set; }

    #endregion
}