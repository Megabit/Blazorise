using System.Text.Json.Serialization;

namespace Blazorise.LottieAnimation;

[JsonConverter(typeof(AnimationSegmentJsonConverter))]
public record AnimationSegment(int FinalFrame, int InitialFrame);