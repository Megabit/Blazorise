namespace Blazorise.SignaturePad;

/// <summary>
/// This class is used to represent the arguments for an event when a stroke begins in a signature pad.
/// </summary>
public class SignaturePadBeginStrokeEventArgs
{
    /// <summary>
    /// Initializes a new instance of the SignaturePadBeginStrokeEventArgs class.
    /// </summary>
    /// <param name="offsetX">The X-axis offset of the stroke.</param>
    /// <param name="offsetY">The Y-axis offset of the stroke.</param>
    public SignaturePadBeginStrokeEventArgs( double offsetX, double offsetY )
    {
        OffsetX = offsetX;
        OffsetY = offsetY;
    }

    /// <summary>
    /// Gets the X-axis offset of the stroke.
    /// </summary>
    public double OffsetX { get; }

    /// <summary>
    /// Gets the Y-axis offset of the stroke.
    /// </summary>
    public double OffsetY { get; }
}