namespace Blazorise.PivotGrid;

/// <summary>
/// Defines a field available for runtime PivotGrid layout customization.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class PivotGridField<TItem> : BasePivotGridField<TItem>
{
    /// <inheritdoc />
    public override PivotGridFieldArea FieldArea => PivotGridFieldArea.Available;
}