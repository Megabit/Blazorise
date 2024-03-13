#region Using directives
#endregion

namespace Blazorise.Tailwind.Components;

public class CarouselSlide : Blazorise.CarouselSlide
{
    /// <summary>
    /// The time it takes to animate the carousel slide transition.
    /// </summary>
    internal protected override int AnimationTime { get; set; } = 700;
}
