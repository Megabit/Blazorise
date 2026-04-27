namespace Blazorise.PivotGrid;

/// <summary>
/// Defines a row dimension in <see cref="PivotGrid{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class PivotGridRow<TItem> : BasePivotGridField<TItem>
{
    /// <inheritdoc />
    public override PivotGridFieldArea FieldArea => PivotGridFieldArea.Row;
}