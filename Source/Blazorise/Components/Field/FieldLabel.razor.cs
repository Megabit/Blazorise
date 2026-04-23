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

    private bool refreshQueued;

    private bool refreshRequestedWhileQueued;

    private bool subscribedToLabelTargetChanged;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if ( ParentField is not null )
        {
            if ( UseAriaLabelledByAttribute )
            {
                ParentField.NotifyFieldLabelInitialized( this );
            }
        }

        UpdateLabelTargetSubscription();
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        UpdateLabelTargetSubscription();
    }

    /// <inheritdoc/>
    protected override Task OnFirstAfterRenderAsync()
    {
        if ( CanUseForAttribute && For is null && ParentField?.LabelTargetElementId is not null )
        {
            QueueRefresh();
        }

        return base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await base.OnAfterRenderAsync( firstRender );

        refreshQueued = false;

        if ( refreshRequestedWhileQueued && !( Disposed || AsyncDisposed ) )
        {
            refreshRequestedWhileQueued = false;
            QueueRefresh();
        }
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing && ParentField is not null )
        {
            if ( subscribedToLabelTargetChanged )
            {
                ParentField.LabelTargetChanged -= OnLabelTargetChanged;
                subscribedToLabelTargetChanged = false;
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
        if ( For is not null )
            return;

        QueueRefresh();
    }

    /// <summary>
    /// Queues a single component refresh for the current render cycle.
    /// </summary>
    private void QueueRefresh()
    {
        if ( Disposed || AsyncDisposed )
            return;

        if ( refreshQueued )
        {
            refreshRequestedWhileQueued = true;
            return;
        }

        refreshQueued = true;

        _ = InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Synchronizes the parent field subscription used for automatic <c>for</c> attribute updates.
    /// </summary>
    private void UpdateLabelTargetSubscription()
    {
        if ( ParentField is null )
            return;

        if ( CanUseForAttribute && !subscribedToLabelTargetChanged )
        {
            ParentField.LabelTargetChanged += OnLabelTargetChanged;
            subscribedToLabelTargetChanged = true;
        }
        else if ( !CanUseForAttribute && subscribedToLabelTargetChanged )
        {
            ParentField.LabelTargetChanged -= OnLabelTargetChanged;
            subscribedToLabelTargetChanged = false;
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the tag name rendered by this component.
    /// </summary>
    protected string ContainerTagName => IsLegend ? "legend" : "label";

    /// <summary>
    /// Gets a value indicating whether the label should render as a legend element.
    /// </summary>
    protected bool IsLegend => ForceLegend || ParentField?.IsGroup == true;

    /// <summary>
    /// Gets a value indicating whether the component always renders as a legend element.
    /// </summary>
    protected virtual bool ForceLegend => false;

    /// <summary>
    /// Gets a value indicating whether the automatic <c>for</c> attribute can be used.
    /// </summary>
    protected bool CanUseForAttribute => UseFieldLabelForAttribute && !IsLegend;

    /// <summary>
    /// Gets the resolved ID of the input element that this label belongs to.
    /// </summary>
    protected string ResolvedFor => CanUseForAttribute
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
    /// Specifies the ID of an element that this label belongs to.
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
    /// Specifies the visibility for screen readers.
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