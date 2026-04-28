namespace Blazorise.PivotGrid;

internal class PivotGridRuntimeField<TItem> : BasePivotGridField<TItem>
{
    public PivotGridRuntimeField( PivotGridFieldArea fieldArea )
    {
        FieldArea = fieldArea;
    }

    public override PivotGridFieldArea FieldArea { get; }
}