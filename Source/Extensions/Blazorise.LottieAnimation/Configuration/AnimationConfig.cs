using Microsoft.JSInterop;

namespace Blazorise.LottieAnimation;

public abstract record AnimationConfig
{
    public string               AssetsPath       { get; init; }
    public bool?                 Autoplay         { get; init; }
    public AnimationSegment     InitialSegment   { get; init; }
    public LoopingConfiguration Loop             { get; init; }
    public string               Name             { get; init; }
    public Renderer              Renderer         { get; init; }
    public BaseRendererConfig   RendererSettings { get; init; }
    
    public DotNetObjectReference<JSInteropCallback<EnteredFrameEventArgs>> EnterFrameCallback { get; init; }
}