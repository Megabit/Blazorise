#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Tooltip Configuration
/// </summary>
public class ChartTooltip
{
    /// <summary>
    /// Are on-canvas tooltips enabled.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Enabled { get; set; }

    /// <summary>
    /// Sets which elements appear in the tooltip.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Mode { get; set; }

    /// <summary>
    /// If true, the tooltip mode applies only when the mouse position intersects with an element. If false, the mode will be applied at all times.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Intersect { get; set; }

    /// <summary>
    /// The mode for positioning the tooltip.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Position { get; set; }

    /// <summary>
    /// Tooltip callbacks.
    /// </summary>
    [JsonPropertyName( "callbacks" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartTooltipCallbacks Callbacks { get; set; }

    /// <summary>
    /// Background color of the tooltip.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
    public IndexableOption<object> BackgroundColor { get; set; }

    /// <summary>
    /// Color of title text.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
    public IndexableOption<object> TitleColor { get; set; }

    /// <summary>
    /// See <see href="https://www.chartjs.org/docs/latest/general/fonts.html">Fonts</see>.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartFont TitleFont { get; set; } = new ChartFont { Weight = "bold" };

    /// <summary>
    /// Horizontal alignment of the title text lines.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string TitleAlign { get; set; }

    /// <summary>
    /// Spacing to add to top and bottom of each title line.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? TitleSpacing { get; set; }

    /// <summary>
    /// Margin to add on bottom of title section.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? TitleMarginBottom { get; set; }

    /// <summary>
    /// Color of body text.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
    public IndexableOption<object> BodyColor { get; set; }

    /// <summary>
    /// See <see href="https://www.chartjs.org/docs/latest/general/fonts.html">Fonts</see>.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartFont BodyFont { get; set; }

    /// <summary>
    /// Horizontal alignment of the body text lines.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BodyAlign { get; set; }

    /// <summary>
    /// Spacing to add to top and bottom of each tooltip item.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? BodySpacing { get; set; }

    /// <summary>
    /// Color of footer text.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
    public IndexableOption<object> FooterColor { get; set; }

    /// <summary>
    /// See <see href="https://www.chartjs.org/docs/latest/general/fonts.html">Fonts</see>.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartFont FooterFont { get; set; }

    /// <summary>
    /// Horizontal alignment of the footer text lines.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string FooterAlign { get; set; }

    /// <summary>
    /// Spacing to add to top and bottom of each footer line.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? FooterSpacing { get; set; }

    /// <summary>
    /// Margin to add before drawing the footer.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? FooterMarginTop { get; set; }

    /// <summary>
    /// Padding inside the tooltip.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Padding { get; set; }

    /// <summary>
    /// Extra distance to move the end of the tooltip arrow away from the tooltip point.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object CaretPadding { get; set; }

    /// <summary>
    /// Size, in px, of the tooltip arrow.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? CaretSize { get; set; }

    /// <summary>
    /// Radius of tooltip corner curves.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? CornerRadius { get; set; }

    /// <summary>
    /// Color to draw behind the colored boxes when multiple items are in the tooltip.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
    public IndexableOption<object> MultiKeyBackground { get; set; }

    /// <summary>
    /// If true, color boxes are shown in the tooltip.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? DisplayColors { get; set; }

    /// <summary>
    /// Width of the color box if displayColors is true.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? BoxWidth { get; set; }

    /// <summary>
    /// Height of the color box if displayColors is true.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? BoxHeight { get; set; }

    /// <summary>
    /// Padding between the color box and the text.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? BoxPadding { get; set; }

    /// <summary>
    /// Use the corresponding point style (from dataset options) instead of color boxes, ex: star, triangle etc. (size is based on the minimum value between boxWidth and boxHeight).
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? UsePointStyle { get; set; }

    /// <summary>
    /// Color of the border.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
    public IndexableOption<object> BorderColor { get; set; }

    /// <summary>
    /// Size of the border.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? BorderWidth { get; set; }

    /// <summary>
    /// true for rendering the tooltip from right to left.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Rtl { get; set; }

    /// <summary>
    /// This will force the text direction 'rtl' or 'ltr on the canvas for rendering the tooltips, regardless of the css specified on the canvas
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string TextDirection { get; set; }

    /// <summary>
    /// Position of the tooltip caret in the X direction.
    /// <list type="bullet">
    ///     <item>
    ///         <term>"left"</term>
    ///         <description>(default)</description>
    ///     </item>
    ///     <item>
    ///         <term>"center"</term>
    ///         <description></description>
    ///     </item>
    ///     <item>
    ///         <term>"right"</term>
    ///         <description></description>
    ///     </item>
    /// </list>
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string XAlign { get; set; }

    /// <summary>
    /// Position of the tooltip caret in the Y direction.
    /// <list type="bullet">
    ///     <item>
    ///         <term>"left"</term>
    ///         <description>(default)</description>
    ///     </item>
    ///     <item>
    ///         <term>"center"</term>
    ///         <description></description>
    ///     </item>
    ///     <item>
    ///         <term>"right"</term>
    ///         <description></description>
    ///     </item>
    /// </list>
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string YAlign { get; set; }
}