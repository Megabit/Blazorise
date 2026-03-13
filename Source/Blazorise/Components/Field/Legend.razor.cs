#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Legend for a <see cref="FieldSet"/> component.
/// </summary>
public partial class Legend : BaseColumnComponent, IDisposable
{
    #region Members

    private bool requiredIndicator;

    private Screenreader screenreader = Screenreader.Always;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ParentFieldSet?.Hook( this );

        base.OnInitialized();

        if ( UseAriaLabelledByAttribute )
        {
            ParentFieldSet?.NotifyLegendInitialized( this );
        }
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentFieldSet?.UnHook( this );

            if ( UseAriaLabelledByAttribute )
            {
                ParentFieldSet?.NotifyLegendRemoved( this );
            }
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Legend( IsHorizontal ) );
        builder.Append( ClassProvider.LegendRequiredIndicator( RequiredIndicator ) );
        builder.Append( ClassProvider.LegendScreenreader( Screenreader ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether the legend is inside of a horizontal <see cref="FieldSet"/>.
    /// </summary>
    protected bool IsHorizontal => ParentFieldSet?.Horizontal == true;

    /// <summary>
    /// Gets a value indicating whether the automatic <c>aria-labelledby</c> integration is enabled.
    /// </summary>
    protected bool UseAriaLabelledByAttribute => Options?.AccessibilityOptions?.UseAriaLabelledByAttribute == true;

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => UseAriaLabelledByAttribute;

    /// <summary>
    /// Gets or sets the reference to the parent <see cref="FieldSet"/> component.
    /// </summary>
    [CascadingParameter] protected FieldSet ParentFieldSet { get; set; }

    /// <summary>
    /// If defined, a required indicator will be shown next to the legend.
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