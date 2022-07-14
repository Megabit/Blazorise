namespace Blazorise.LoadingIndicator
{
    /// <summary>
    /// A service to control LoadingIndicator components
    /// </summary>
    public class LoadingIndicatorService
    {
        internal delegate void StateChanged(bool val);
        internal event StateChanged BusyChanged;
        internal event StateChanged LoadedChanged;
        
        /// <summary>
        /// Set Busy state
        /// </summary>
        /// <param name="val">true or false</param>
        public void SetBusy(bool val) => BusyChanged.Invoke(val);

        /// <summary>
        /// Set Loaded state
        /// </summary>
        /// <param name="val">true or false</param>
        public void SetLoaded(bool val) => LoadedChanged.Invoke(val);
    }
}