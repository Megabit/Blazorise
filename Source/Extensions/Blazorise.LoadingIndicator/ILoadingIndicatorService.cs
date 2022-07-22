namespace Blazorise.LoadingIndicator
{
    public interface ILoadingIndicatorService
    {
        bool? Busy { get; }
        bool? Loaded { get; }

        void Hide();
        void SetLoaded( bool value );
        void Show();

        internal void Subscribe( LoadingIndicator indicator );
        internal void Unsubscribe( LoadingIndicator indicator );
    }
}