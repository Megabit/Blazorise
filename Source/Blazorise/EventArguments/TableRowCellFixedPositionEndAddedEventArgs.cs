#region Using directives
#endregion

namespace Blazorise;

public partial class TableRow
{
    /// <summary>
    /// Triggers when the width of the cell with TableColumnFixedPosition.End changes.
    /// </summary>
    public class TableRowCellFixedPositionEndAddedEventArgs
    {
        /// <summary>
        /// The width of the cell with TableColumnFixedPosition.End.
        /// </summary>
        public double Width { get; set; }
    }

}