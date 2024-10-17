#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Wrapper for input components.
/// </summary>
public partial class Control : BaseComponent
{
    #region Members

    private bool inline;

    private ControlRole role = ControlRole.None;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ControlCheck( Role ) );
        builder.Append( ClassProvider.ControlRadio( Role ) );
        builder.Append( ClassProvider.ControlSwitch( Role ) );
        builder.Append( ClassProvider.ControlFile( Role ) );
        builder.Append( ClassProvider.ControlText( Role ) );
        builder.Append( ClassProvider.ControlInline( Role, Inline ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Determines if the check or radio control will be inlined.
    /// </summary>
    [Parameter]
    public bool Inline
    {
        get => inline;
        set
        {
            inline = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Sets the role that affects the behaviour of the control container.
    /// </summary>
    [Parameter]
    public ControlRole Role
    {
        get => role;
        set
        {
            role = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Control"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}