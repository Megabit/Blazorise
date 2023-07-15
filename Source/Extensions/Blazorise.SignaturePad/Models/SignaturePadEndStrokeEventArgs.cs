namespace Blazorise.SignaturePad;

/// <summary>
/// This class is used to represent the arguments for an event when a stroke ends in a signature pad.
/// </summary>
public class SignaturePadEndStrokeEventArgs
{
    /// <summary>
    /// Initializes a new instance of the SignaturePadEndStrokeEventArgs class.
    /// </summary>
    /// <param name="data">The byte array representing the stroke data.</param>
    /// <param name="dataUrl">The data URL of the stroke.</param>
    /// <param name="offsetX">The X-axis offset of the stroke.</param>
    /// <param name="offsetY">The Y-axis offset of the stroke.</param>
    public SignaturePadEndStrokeEventArgs( byte[] data, string dataUrl, double offsetX, double offsetY )
    {
        Data = data;
        DataUrl = dataUrl;
        OffsetX = offsetX;
        OffsetY = offsetY;
    }

    /// <summary>
    /// Gets the byte array representing the stroke data.
    /// </summary>
    public byte[] Data { get; }

    /// <summary>
    /// Gets the data URL of the stroke.
    /// </summary>
    public string DataUrl { get; }

    /// <summary>
    /// Gets the X-axis offset of the stroke.
    /// </summary>
    public double OffsetX { get; }

    /// <summary>
    /// Gets the Y-axis offset of the stroke.
    /// </summary>
    public double OffsetY { get; }
}