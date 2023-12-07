namespace Blazorise;

public partial class TableRow
{
    /// <summary>
    /// Triggers when the width of the cell with TableColumnFixedPosition.End changes.
    /// </summary>
    public class TableRowCellFixedPositionEndAddedEventArgs
    {
        /// <summary>
        /// The width of the cell with <see cref="TableColumnFixedPosition.End"/> defined.
        /// </summary>
        public double Width { get; set; }
    }
}