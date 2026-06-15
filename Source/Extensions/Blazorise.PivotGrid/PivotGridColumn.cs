namespace Blazorise.PivotGrid;

/// <summary>
/// Defines a column dimension in <see cref="PivotGrid{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class PivotGridColumn<TItem> : BasePivotGridField<TItem>
{
    /// <inheritdoc />
    public override PivotGridFieldArea FieldArea => PivotGridFieldArea.Column;
}