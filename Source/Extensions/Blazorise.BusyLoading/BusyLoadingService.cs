namespace Blazorise.BusyLoading
{
    /// <summary>
    /// A service to control BusyLoading components
    /// </summary>
    public class BusyLoadingService
    {
        internal delegate void StateChanged(bool val);
        internal event StateChanged BusyChanged;
        internal event StateChanged LoadedChanged;
        
        /// <summary>
        /// Set IsBusy state
        /// </summary>
        /// <param name="val">true or false</param>
        public void Busy(bool val) => BusyChanged.Invoke(val);

        /// <summary>
        /// Set IsLoaded state
        /// </summary>
        /// <param name="val">true or false</param>
        public void Loaded(bool val) => LoadedChanged.Invoke(val);
    }
}