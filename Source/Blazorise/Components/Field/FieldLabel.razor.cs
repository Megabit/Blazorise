#region Using directives
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Label for a <see cref="Field"/> component.
/// </summary>
public partial class FieldLabel : BaseSizableFieldComponent
{
    #region Members

    private bool requiredIndicator;

    private Screenreader screenreader = Screenreader.Always;

    private FieldLabelClasses classes;

    private FieldLabelStyles styles;

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

    /// <inheritdoc/>
    protected override void BuildCustomClasses( ClassBuilder builder )
    {
        builder.Append( Classes?.Main );
    }

    /// <inheritdoc/>
    protected override void BuildCustomStyles( StyleBuilder builder )
    {
        builder.Append( Styles?.Main );
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

    /// <summary>
    /// Custom CSS class names for field label elements.
    /// </summary>
    [Parameter]
    public FieldLabelClasses Classes
    {
        get => classes;
        set
        {
            if ( classes.IsEqual( value ) )
                return;

            classes = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Custom inline styles for field label elements.
    /// </summary>
    [Parameter]
    public FieldLabelStyles Styles
    {
        get => styles;
        set
        {
            if ( styles.IsEqual( value ) )
                return;

            styles = value;

            DirtyStyles();
        }
    }

    #endregion
}