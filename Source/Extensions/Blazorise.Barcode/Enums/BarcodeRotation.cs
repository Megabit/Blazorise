namespace Blazorise.Barcode;

/// <summary>
/// Defines the barcode rotation.
/// </summary>
public enum BarcodeRotation
{
    /// <summary>
    /// No rotation.
    /// </summary>
    None,

    /// <summary>
    /// Rotates clockwise by 90 degrees.
    /// </summary>
    Right,

    /// <summary>
    /// Rotates counter-clockwise by 90 degrees.
    /// </summary>
    Left,

    /// <summary>
    /// Rotates by 180 degrees.
    /// </summary>
    Inverted
}