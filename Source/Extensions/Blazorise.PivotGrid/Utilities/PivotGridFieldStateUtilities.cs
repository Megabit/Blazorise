namespace Blazorise.PivotGrid.Utilities;

internal static class PivotGridFieldStateUtilities
{
    internal static PivotGridFieldState Clone( PivotGridFieldState state )
        => new()
        {
            Field = state.Field,
            Caption = state.Caption,
            FieldType = state.FieldType,
            Area = state.Area,
            Aggregate = state.Aggregate,
            FilterValueKey = state.FilterValueKey
        };
}