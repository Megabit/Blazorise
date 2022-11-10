namespace Blazorise.Cropper;

/// <summary>
/// Cropper aspect ratio.
/// </summary>
public record struct CropperAspectRatio( double? Value )
{
    /// <summary>
    /// Create a new aspect ratio based on width and height
    /// </summary>
    /// <param name="width">ratio width</param>
    /// <param name="height">ratio height</param>
    public CropperAspectRatio( int width, int height )
        : this( (double)width / height )
    {
    }

    /// <summary>
    /// 16:9 aspect ratio
    /// </summary>
    public static readonly CropperAspectRatio Is16x9 = new( 16, 9 );

    /// <summary>
    /// 4:3 aspect ratio
    /// </summary>
    public static readonly CropperAspectRatio Is4x3 = new( 4, 3 );

    /// <summary>
    /// 1:1 aspect ratio
    /// </summary>
    public static readonly CropperAspectRatio Is1x1 = new( 1 );

    /// <summary>
    /// 2:3 aspect ratio
    /// </summary>
    public static readonly CropperAspectRatio Is2x3 = new( 2, 3 );

    /// <summary>
    /// Free aspect ratio
    /// </summary>
    public static readonly CropperAspectRatio IsFree = new( null );
}