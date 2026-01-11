#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Label for a <see cref="Field"/> component.
/// </summary>
public partial class FieldLabel : BaseSizableFieldComponent<FieldLabelClasses, FieldLabelStyles>
{
    #region Members

    private bool requiredIndicator;

    private Screenreader screenreader = Screenreader.Always;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.FieldLabel( IsHorizontal ) );
        builder.Append( ClassProvider.FieldLabelRequiredIndicator( RequiredIndicator ) );
        builder.Append( ClassProvider.FieldLabelScreenreader( Screenreader ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the ID of an element that this label belongs to.
    /// </summary>
    [Parameter] public string For { get; set; }

    /// <summary>
    /// If defined, a required indicator will be shown next to the label.
    /// </summary>
    [Parameter]
    public bool RequiredIndicator
    {
        get => requiredIndicator;
        set
        {
            requiredIndicator = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the visibility for screen readers.
    /// </summary>
    [Parameter]
    public Screenreader Screenreader
    {
        get => screenreader;
        set
        {
            screenreader = value;

            DirtyClasses();
        }
    }

    #endregion
}