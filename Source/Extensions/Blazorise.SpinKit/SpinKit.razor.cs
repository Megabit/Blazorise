#region Using directives
using System.Text;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.SpinKit;

/// <summary>
/// Simple loading spinner animated with CSS. 
/// </summary>
public partial class SpinKit : ComponentBase
{
    #region Methods

    private static string ToSpinKitName( SpinKitType spinKitType )
    {
        return spinKitType switch
        {
            SpinKitType.Plane => "plane",
            SpinKitType.Chase => "chase",
            SpinKitType.Bounce => "bounce",
            SpinKitType.Wave => "wave",
            SpinKitType.Pulse => "pulse",
            SpinKitType.Flow => "flow",
            SpinKitType.Swing => "swing",
            SpinKitType.Circle => "circle",
            SpinKitType.CircleFade => "circle-fade",
            SpinKitType.Grid => "grid",
            SpinKitType.Fold => "fold",
            SpinKitType.Wander => "wander",
            _ => null,
        };
    }

    private static string ToSpinKitItemName( SpinKitType spinKitType )
    {
        return spinKitType switch
        {
            SpinKitType.Chase or SpinKitType.Bounce or SpinKitType.Flow or SpinKitType.Swing or SpinKitType.Circle or SpinKitType.CircleFade => "dot",
            SpinKitType.Wave => "rect",
            SpinKitType.Grid or SpinKitType.Fold or SpinKitType.Wander => "cube",
            _ => null,
        };
    }

    private static int SpinKitItemCount( SpinKitType spinKitType )
    {
        return spinKitType switch
        {
            SpinKitType.Chase => 6,
            SpinKitType.Bounce => 2,
            SpinKitType.Wave => 5,
            SpinKitType.Flow => 3,
            SpinKitType.Swing => 2,
            SpinKitType.Circle or SpinKitType.CircleFade => 12,
            SpinKitType.Grid => 9,
            SpinKitType.Fold => 4,
            SpinKitType.Wander => 3,
            _ => 0,
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the outer spinner classnames.
    /// </summary>
    protected string WrapperClassNames
    {
        get
        {
            var sb = new StringBuilder( $"sk-{ToSpinKitName( Type )}" );

            if ( Centered )
                sb.Append( " sk-center" );

            return sb.ToString();
        }
    }

    /// <summary>
    /// Gets the outer spinner styles.
    /// </summary>
    protected string WrapperStyleNames
    {
        get
        {
            var sb = new StringBuilder();

            if ( !string.IsNullOrEmpty( Color ) )
                sb.Append( $"--sk-color: {Color};" );

            if ( !string.IsNullOrEmpty( Size ) )
                sb.Append( $"--sk-size: {Size};" );

            return sb.ToString();
        }
    }

    /// <summary>
    /// Gets the inner spinner classnames.
    /// </summary>
    protected string ItemClassNames
    {
        get
        {
            var itemName = ToSpinKitItemName( Type );

            if ( string.IsNullOrEmpty( itemName ) )
                return $"sk-{ToSpinKitName( Type )}";

            return $"sk-{ToSpinKitName( Type )}-{itemName}";
        }
    }

    /// <summary>
    /// Gets or sets the spinner type.
    /// </summary>
    [Parameter] public SpinKitType Type { get; set; } = SpinKitType.Plane;

    /// <summary>
    /// Gets or sets the spinner color.
    /// </summary>
    [Parameter] public string Color { get; set; }

    /// <summary>
    /// Gets or sets the spinner size.
    /// </summary>
    [Parameter] public string Size { get; set; }

    /// <summary>
    /// Position the spinner to the center of it's container.
    /// </summary>
    [Parameter] public bool Centered { get; set; }

    #endregion
}