#region Using directives
using System.ComponentModel;
using System.Reflection;
using Blazorise.Licensing;
#endregion

namespace Blazorise
{
    internal class BlazoriseLicenseProvider
    {
        private static readonly Assembly CurrentAssembly = typeof( BlazoriseLicenseProvider ).Assembly;

        private readonly BlazoriseOptions options;

        private readonly BackgroundWorker backgroundWorker;

        public BlazoriseLicenseProvider( BlazoriseOptions options )
        {
            this.options = options;

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;

            backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_DoWork( object sender, DoWorkEventArgs e )
        {
            if ( string.IsNullOrWhiteSpace( options.LicenseKey ) )
            {
                Result = BlazoriseLicenseResult.Community;
                return;
            }

            try
            {
                Result = SerialNumber.IsValid( options.LicenseKey, CurrentAssembly )
                    ? BlazoriseLicenseResult.Licensed
                    : BlazoriseLicenseResult.Trial;
            }
            catch
            {
                Result = BlazoriseLicenseResult.Trial;
            }
        }

        public BlazoriseLicenseResult Result { get; private set; } = BlazoriseLicenseResult.Initializing;
    }
}
