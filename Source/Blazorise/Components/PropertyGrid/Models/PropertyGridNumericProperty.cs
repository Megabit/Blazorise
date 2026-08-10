#region Using directives
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise;

/// <summary>
/// Describes a numeric property.
/// </summary>
public sealed class PropertyGridNumericProperty<TValue> : PropertyGridProperty<TValue>
{
    #region Constructors

    /// <summary>
    /// Initializes a numeric property.
    /// </summary>
    public PropertyGridNumericProperty( string key, string label, TValue value )
        : base( key, label, value )
    {
    }

    #endregion

    #region Methods

    internal override void RenderAtomicComponent( RenderTreeBuilder builder, PropertyGridView owner, bool selected, string ariaDescribedBy )
    {
        builder.OpenComponent<PropertyGridNumericItem<TValue>>( 0 );
        owner.AddAtomicComponentParameters( builder, this, selected, ariaDescribedBy );
        builder.AddAttribute( 15, nameof( PropertyGridNumericItem<TValue>.Min ), Min );
        builder.AddAttribute( 16, nameof( PropertyGridNumericItem<TValue>.Max ), Max );
        builder.AddAttribute( 17, nameof( PropertyGridNumericItem<TValue>.Step ), Step );
        builder.AddAttribute( 18, nameof( PropertyGridNumericItem<TValue>.Immediate ), Immediate );
        builder.CloseComponent();
    }

    #endregion

    #region Properties

    internal override PropertyGridEditorType EditorType => PropertyGridEditorType.Numeric;

    /// <summary>
    /// Gets or sets the minimum accepted value.
    /// </summary>
    public double Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum accepted value.
    /// </summary>
    public double? Max { get; set; }

    /// <summary>
    /// Gets or sets the numeric increment.
    /// </summary>
    public decimal? Step { get; set; } = 1m;

    /// <summary>
    /// Gets or sets whether changes are reported immediately.
    /// </summary>
    public bool Immediate { get; set; } = true;

    #endregion
}