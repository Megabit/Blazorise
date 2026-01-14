#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Sets the field help-text positioned bellow the field.
/// </summary>
public partial class FieldHelp : BaseComponent, IDisposable
{
    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        ParentField?.NotifyFieldHelpInitialized( this );
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentField?.NotifyFieldHelpRemoved( this );
        }

        base.Dispose( disposing );
    }

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

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets or sets the reference to the parent <see cref="FieldBody"/> component.
    /// </summary>
    [CascadingParameter] protected FieldBody ParentFieldBody { get; set; }

    /// <summary>
    /// Gets or sets the reference to the parent <see cref="Field"/> component.
    /// </summary>
    [CascadingParameter] protected Field ParentField { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="FieldHelp"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}