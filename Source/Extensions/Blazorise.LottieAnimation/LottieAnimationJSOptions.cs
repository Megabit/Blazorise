namespace Blazorise.LottieAnimation;

public class LottieAnimationInitializeJSOptions
{
    public string Path { get; set; }
    public LoopConfiguration Loop { get; set; }
    public bool Autoplay { get; set; }
    public LottieAnimationRenderer Renderer { get; set; }
    public LottieAnimationDirection Direction { get; set; }
    public double Speed { get; set; }
    public bool SendCurrentFrame { get; set; }
}