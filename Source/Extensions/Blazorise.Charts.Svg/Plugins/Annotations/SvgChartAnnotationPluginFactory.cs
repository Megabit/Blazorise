namespace Blazorise.Charts.Svg;

internal static class SvgChartAnnotationPluginFactory
{
    #region Methods

    public static ISvgChartPlugin Create( SvgChartAnnotationOptions annotation )
    {
        return annotation switch
        {
            SvgChartLineAnnotationOptions lineAnnotation => SvgChartLineAnnotation.Create( lineAnnotation ),
            SvgChartBoxAnnotationOptions boxAnnotation => SvgChartBoxAnnotation.Create( boxAnnotation ),
            SvgChartLabelAnnotationOptions labelAnnotation => SvgChartLabelAnnotation.Create( labelAnnotation ),
            SvgChartPointAnnotationOptions pointAnnotation => SvgChartPointAnnotation.Create( pointAnnotation ),
            SvgChartEllipseAnnotationOptions ellipseAnnotation => SvgChartEllipseAnnotation.Create( ellipseAnnotation ),
            _ => null
        };
    }

    #endregion
}