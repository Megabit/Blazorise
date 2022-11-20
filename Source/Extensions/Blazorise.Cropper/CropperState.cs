using Blazorise.Infrastructure;

namespace Blazorise.Cropper
{
    /// <summary>
    /// Provides a shared state and syncronization between the cropper and cropper viewer.
    /// </summary>
    public class CropperState
    {
        internal EventCallbackSubscribable<Cropper> CropperInitialized { get; } = new();
    }
}
