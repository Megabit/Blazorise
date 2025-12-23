#region Using directives
using Blazorise;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.SpinKit;

/// <summary>
/// Simple loading spinner animated with CSS. 
/// </summary>
public partial class SpinKit : BaseComponent
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

    private static string ToSpinKitSizeName( Size size )
    {
        return size switch
        {
            Size.ExtraSmall => "xs",
            Size.Small => "sm",
            Size.Medium => "md",
            Size.Large => "lg",
            Size.ExtraLarge => "xl",
            _ => null,
        };
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( $"sk-{ToSpinKitName( Type )}" );

        if ( Centered )
            builder.Append( "sk-center" );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( !string.IsNullOrEmpty( HexColor ) )
        {
            builder.Append( $"--sk-color: {HexColor};" );
        }
        else if ( Color is not null && Color != Blazorise.Color.Default )
        {
            builder.Append( $"--sk-color: var(--b-spinkit-color-{Color.Name});" );
        }

        if ( Size != Blazorise.Size.Default )
        {
            var sizeName = ToSpinKitSizeName( Size );

            if ( !string.IsNullOrEmpty( sizeName ) )
                builder.Append( $"--sk-size: var(--b-spinkit-size-{sizeName});" );
        }

        base.BuildStyles( builder );
    }

    #endregion

    #region Properties

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
    [Parameter]
    public SpinKitType Type
    {
        get => type;
        set
        {
            type = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets the spinner color variant.
    /// </summary>
    [Parameter]
    public Color Color
    {
        get => color;
        set
        {
            color = value;

            DirtyStyles();
        }
    }

    /// <summary>
    /// Gets or sets the spinner custom hex color that overrides the color variant.
    /// </summary>
    [Parameter]
    public string HexColor
    {
        get => hexColor;
        set
        {
            hexColor = value;

            DirtyStyles();
        }
    }

    /// <summary>
    /// Gets or sets the spinner size.
    /// </summary>
    [Parameter]
    public Size Size
    {
        get => size;
        set
        {
            size = value;

            DirtyStyles();
        }
    }

    /// <summary>
    /// Position the spinner to the center of it's container.
    /// </summary>
    [Parameter]
    public bool Centered
    {
        get => centered;
        set
        {
            centered = value;

            DirtyClasses();
        }
    }

    #endregion

    #region Members

    private SpinKitType type = SpinKitType.Plane;

    private Color color = Blazorise.Color.Default;

    private string hexColor;

    private Size size = Blazorise.Size.Default;

    private bool centered;

    #endregion
}