#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Label for a <see cref="Fields"/> component.
/// </summary>
public partial class FieldsLabel : BaseColumnComponent, IDisposable
{
    #region Members

    private bool requiredIndicator;

    private Screenreader screenreader = Screenreader.Always;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if ( UseAriaLabelledByAttribute )
        {
            ParentFields?.NotifyFieldsLabelInitialized( this );
        }
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing && UseAriaLabelledByAttribute )
        {
            ParentFields?.NotifyFieldsLabelRemoved( this );
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.FieldLabel( false ) );
        builder.Append( ClassProvider.FieldLabelRequiredIndicator( RequiredIndicator ) );
        builder.Append( ClassProvider.FieldLabelScreenreader( Screenreader ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the tag name rendered by this component.
    /// </summary>
    protected string ContainerTagName => ParentFields?.Group == true ? "legend" : "label";

    /// <summary>
    /// Gets a value indicating whether the automatic <c>aria-labelledby</c> integration is enabled.
    /// </summary>
    protected bool UseAriaLabelledByAttribute => Options?.AccessibilityOptions?.UseAriaLabelledByAttribute == true;

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => UseAriaLabelledByAttribute;

    /// <summary>
    /// Gets or sets the reference to the parent <see cref="Fields"/> component.
    /// </summary>
    [CascadingParameter] protected Fields ParentFields { get; set; }

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
    /// Holds the information about the Blazorise global options.
    /// </summary>
    [Inject] protected BlazoriseOptions Options { get; set; }

    #endregion
}