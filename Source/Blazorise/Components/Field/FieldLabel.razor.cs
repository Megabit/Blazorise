#region Using directives
using System.Threading.Tasks;
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
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if ( ParentField is not null )
        {
            if ( UseFieldLabelForAttribute )
            {
                ParentField.LabelTargetChanged += OnLabelTargetChanged;
            }

            if ( UseAriaLabelledByAttribute )
            {
                ParentField.NotifyFieldLabelInitialized( this );
            }
        }
    }

    /// <inheritdoc/>
    protected override Task OnFirstAfterRenderAsync()
    {
        if ( UseFieldLabelForAttribute && For is null && ParentField?.LabelTargetElementId is not null )
        {
            return InvokeAsync( StateHasChanged );
        }

        return base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing && ParentField is not null )
        {
            if ( UseFieldLabelForAttribute )
            {
                ParentField.LabelTargetChanged -= OnLabelTargetChanged;
            }

            if ( UseAriaLabelledByAttribute )
            {
                ParentField.NotifyFieldLabelRemoved( this );
            }
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.FieldLabel( IsHorizontal ) );
        builder.Append( ClassProvider.FieldLabelRequiredIndicator( RequiredIndicator ) );
        builder.Append( ClassProvider.FieldLabelScreenreader( Screenreader ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Handles parent field label target changes.
    /// </summary>
    private void OnLabelTargetChanged()
    {
        InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the resolved ID of the input element that this label belongs to.
    /// </summary>
    protected string ResolvedFor => UseFieldLabelForAttribute
        ? For ?? ParentField?.LabelTargetElementId
        : null;

    /// <summary>
    /// Gets a value indicating whether the automatic <c>for</c> attribute integration is enabled.
    /// </summary>
    protected bool UseFieldLabelForAttribute => Options?.AccessibilityOptions?.UseLabelForAttribute == true;

    /// <summary>
    /// Gets a value indicating whether the automatic <c>aria-labelledby</c> integration is enabled.
    /// </summary>
    protected bool UseAriaLabelledByAttribute => Options?.AccessibilityOptions?.UseAriaLabelledByAttribute == true;

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

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => UseAriaLabelledByAttribute;

    /// <summary>
    /// Holds the information about the Blazorise global options.
    /// </summary>
    [Inject] protected BlazoriseOptions Options { get; set; }

    #endregion
}