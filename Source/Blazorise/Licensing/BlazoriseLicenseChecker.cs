namespace Blazorise
{
    internal class BlazoriseLicenseChecker
    {
        private readonly BlazoriseLicenseProvider blazoriseLicenseProvider;

        private bool rendered;

        public BlazoriseLicenseChecker( BlazoriseLicenseProvider blazoriseLicenseProvider )
        {
            this.blazoriseLicenseProvider = blazoriseLicenseProvider;
        }

        public bool ShouldPrint()
        {
            if ( blazoriseLicenseProvider.Result == BlazoriseLicenseResult.Initializing )
                return false;

            if ( !rendered )
            {
                rendered = true;

                return blazoriseLicenseProvider.Result != BlazoriseLicenseResult.Licensed;
            }

            return false;
        }
    }
}
