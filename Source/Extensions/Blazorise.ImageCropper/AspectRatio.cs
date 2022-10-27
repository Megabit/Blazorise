namespace Blazorise.ImageCropper;

/// <summary>
/// Cropper aspect ratio
/// </summary>
public record struct AspectRatio( double? Value )
{
    /// <summary>
    /// Create a new aspect ratio based on width and height
    /// </summary>
    /// <param name="width">ratio width</param>
    /// <param name="height">ratio height</param>
    public AspectRatio( int width, int height ) : this( (double)width / height )
    {
    }

    /// <summary>
    /// 16:9 aspect ratio
    /// </summary>
    public static AspectRatio Ratio16_9 = new( 16, 9 );

    /// <summary>
    /// 4:3 aspect ratio
    /// </summary>
    public static AspectRatio Ratio4_3 = new( 4, 3 );

    /// <summary>
    /// 1:1 aspect ratio
    /// </summary>
    public static AspectRatio Ratio1_1 = new( 1 );

    /// <summary>
    /// 2:3 aspect ratio
    /// </summary>
    public static AspectRatio Ratio2_3 = new( 2, 3 );

    /// <summary>
    /// Free aspect ratio
    /// </summary>
    public static AspectRatio RatioFree = new( null );
}