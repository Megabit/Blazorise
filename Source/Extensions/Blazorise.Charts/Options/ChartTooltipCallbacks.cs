#region Using directives
using System;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Tooltip callbacks.
/// </summary>
public class ChartTooltipCallbacks
{
    /// <summary>
    /// Returns the text to render before the title.
    /// </summary>
    [JsonPropertyName( "beforeTitle" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( LambdaConverter<Func<ChartTooltipItemContext[], string>> ) )]
    public Expression<Func<ChartTooltipItemContext[], string>> BeforeTitle { get; set; }

    /// <summary>
    /// Returns text to render as the title of the tooltip.
    /// </summary>
    [JsonPropertyName( "title" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( LambdaConverter<Func<ChartTooltipItemContext[], string>> ) )]
    public Expression<Func<ChartTooltipItemContext[], string>> Title { get; set; }

    /// <summary>
    /// Returns text to render after the title.
    /// </summary>
    [JsonPropertyName( "afterTitle" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( LambdaConverter<Func<ChartTooltipItemContext[], string>> ) )]
    public Expression<Func<ChartTooltipItemContext[], string>> AfterTitle { get; set; }

    /// <summary>
    /// Returns text to render before the body section.
    /// </summary>
    [JsonPropertyName( "beforeBody" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( LambdaConverter<Func<ChartTooltipItemContext[], string>> ) )]
    public Expression<Func<ChartTooltipItemContext[], string>> BeforeBody { get; set; }

    /// <summary>
    /// Returns text to render before an individual label. This will be called for each item in the tooltip.
    /// </summary>
    [JsonPropertyName( "beforeLabel" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( LambdaConverter<Func<ChartTooltipItemContext, string>> ) )]
    public Expression<Func<ChartTooltipItemContext, string>> BeforeLabel { get; set; }

    /// <summary>
    /// Returns text to render for an individual item in the tooltip. <see href="https://www.chartjs.org/docs/3.7.1/configuration/tooltip.html#label-callback">more...</see>
    /// </summary>
    [JsonPropertyName( "label" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( LambdaConverter<Func<ChartTooltipItemContext, string>> ) )]
    public Expression<Func<ChartTooltipItemContext, string>> Label { get; set; }

    /// <summary>
    /// Returns the colors for the text of the label for the tooltip item. <see href="https://www.chartjs.org/docs/3.7.1/configuration/tooltip.html#label-color-callback">more...</see>
    /// </summary>
    [JsonPropertyName( "labelColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( LambdaConverter<Func<ChartTooltipItemContext, string>> ) )]
    public Expression<Func<ChartTooltipItemContext, string>> LabelColor { get; set; }

    /// <summary>
    /// Returns the colors for the text of the label for the tooltip item.
    /// </summary>
    [JsonPropertyName( "labelTextColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( LambdaConverter<Func<ChartTooltipItemContext, string>> ) )]
    public Expression<Func<ChartTooltipItemContext, string>> LabelTextColor { get; set; }

    /// <summary>
    /// eturns the point style to use instead of color boxes if usePointStyle is true (object with values pointStyle and rotation). Default implementation uses the point style from the dataset points. <see href="https://www.chartjs.org/docs/3.7.1/configuration/tooltip.html#label-point-style-callback">more..</see>
    /// </summary>
    [JsonPropertyName( "labelPointStyle" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( LambdaConverter<Func<ChartTooltipItemContext, ChartPointStyle>> ) )]
    public Expression<Func<ChartTooltipItemContext, ChartPointStyle>> LabelPointStyle { get; set; }

    /// <summary>
    /// Returns text to render after an individual label.
    /// </summary>
    [JsonPropertyName( "afterLabel" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( LambdaConverter<Func<ChartTooltipItemContext, string>> ) )]
    public Expression<Func<ChartTooltipItemContext, string>> AfterLabel { get; set; }

    /// <summary>
    /// Returns text to render after the body section.
    /// </summary>
    [JsonPropertyName( "afterBody" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( LambdaConverter<Func<ChartTooltipItemContext[], string>> ) )]
    public Expression<Func<ChartTooltipItemContext[], string>> AfterBody { get; set; }

    /// <summary>
    /// Returns text to render before the footer section.
    /// </summary>
    [JsonPropertyName( "beforeFooter" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( LambdaConverter<Func<ChartTooltipItemContext[], string>> ) )]
    public Expression<Func<ChartTooltipItemContext[], string>> BeforeFooter { get; set; }

    /// <summary>
    /// Returns text to render as the footer of the tooltip.
    /// </summary>
    [JsonPropertyName( "footer" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( LambdaConverter<Func<ChartTooltipItemContext[], string>> ) )]
    public Expression<Func<ChartTooltipItemContext[], string>> Footer { get; set; }

    /// <summary>
    /// Text to render after the footer section.
    /// </summary>
    [JsonPropertyName( "afterFooter" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( LambdaConverter<Func<ChartTooltipItemContext[], string>> ) )]
    public Expression<Func<ChartTooltipItemContext[], string>> AfterFooter { get; set; }
}