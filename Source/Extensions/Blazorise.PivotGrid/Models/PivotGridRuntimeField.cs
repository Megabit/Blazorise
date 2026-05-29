namespace Blazorise.PivotGrid;

internal class PivotGridRuntimeField<TItem> : BasePivotGridField<TItem>
{
    public PivotGridRuntimeField( PivotGridFieldArea fieldArea, BasePivotGridField<TItem> source, PivotGridFieldState state )
    {
        FieldArea = fieldArea;
        ApplyRuntimeMetadata( source, state );
    }

    public override PivotGridFieldArea FieldArea { get; }
}