#region Using directives
using System;
#endregion

namespace Blazorise.Charts.Svg;

internal readonly record struct SvgChartStreamingAnimation( bool Enabled, double OffsetX, TimeSpan Duration, int Version, string Style );