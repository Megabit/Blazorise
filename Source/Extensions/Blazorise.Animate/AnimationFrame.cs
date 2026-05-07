namespace Blazorise.Animate;

/// <summary>
/// Defines a single animation keyframe.
/// </summary>
public class AnimationFrame
{
    /// <summary>
    /// Gets or sets the element opacity.
    /// </summary>
    public double? Opacity { get; set; }

    /// <summary>
    /// Gets or sets the horizontal translation.
    /// </summary>
    public object X { get; set; }

    /// <summary>
    /// Gets or sets the vertical translation.
    /// </summary>
    public object Y { get; set; }

    /// <summary>
    /// Gets or sets the scale transform.
    /// </summary>
    public double? Scale { get; set; }

    /// <summary>
    /// Gets or sets the transform perspective.
    /// </summary>
    public object TransformPerspective { get; set; }

    /// <summary>
    /// Gets or sets the rotation transform.
    /// </summary>
    public object Rotate { get; set; }

    /// <summary>
    /// Gets or sets the X-axis rotation transform.
    /// </summary>
    public object RotateX { get; set; }

    /// <summary>
    /// Gets or sets the Y-axis rotation transform.
    /// </summary>
    public object RotateY { get; set; }

    /// <summary>
    /// Gets or sets the Z-axis rotation transform.
    /// </summary>
    public object RotateZ { get; set; }

    /// <summary>
    /// Gets or sets a full transform value.
    /// </summary>
    public string Transform { get; set; }
}