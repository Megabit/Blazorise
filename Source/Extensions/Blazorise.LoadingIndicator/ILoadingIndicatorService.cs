using System.Threading.Tasks;

namespace Blazorise.LoadingIndicator
{
    public interface ILoadingIndicatorService
    {
        bool? Busy { get; }
        bool? Loaded { get; }

        Task Hide();
        Task SetLoaded( bool value );
        Task Show();

        internal void Subscribe( LoadingIndicator indicator );
        internal void Unsubscribe( LoadingIndicator indicator );
    }
}