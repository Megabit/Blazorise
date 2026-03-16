#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Compatibility wrapper that renders a grouped <see cref="FieldLabel"/>.
/// </summary>
public partial class Legend : FieldLabel
{
    /// <inheritdoc/>
    protected override bool ForceLegend => true;

    #region Members

    private bool requiredIndicator;

    private Screenreader screenreader = Screenreader.Always;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether the legend is inside of a horizontal <see cref="FieldSet"/>.
    /// </summary>
    protected new bool IsHorizontal => ParentFieldSet?.Horizontal == true;

    /// <summary>
    /// Gets a value indicating whether the automatic <c>aria-labelledby</c> integration is enabled.
    /// </summary>
    protected new bool UseAriaLabelledByAttribute => base.UseAriaLabelledByAttribute;

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => base.ShouldAutoGenerateId;

    /// <summary>
    /// Gets or sets the reference to the parent <see cref="FieldSet"/> component.
    /// </summary>
    [CascadingParameter] protected FieldSet ParentFieldSet { get; set; }

    /// <summary>
    /// If defined, a required indicator will be shown next to the legend.
    /// </summary>
    [Parameter]
    public new bool RequiredIndicator
    {
        get => base.RequiredIndicator;
        set => base.RequiredIndicator = value;
    }

    /// <summary>
    /// Defines the visibility for screen readers.
    /// </summary>
    [Parameter]
    public new Screenreader Screenreader
    {
        get => base.Screenreader;
        set => base.Screenreader = value;
    }

    /// <summary>
    /// Holds the information about the Blazorise global options.
    /// </summary>
    [Inject] protected BlazoriseOptions Options { get; set; }

    #endregion
}