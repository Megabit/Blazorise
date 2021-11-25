#region Using directives
using System.Runtime.Serialization;
#endregion

namespace Blazorise.Charts
{
    [DataContract]
    public class RadarChartOptions : ChartOptions
    {
        // Unlike other charts, the radar chart has no chart specific options.
    }
}