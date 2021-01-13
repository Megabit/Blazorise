#region Using directives
using System;
using System.Globalization;
#endregion

namespace Blazorise.Localization
{
    public class TextLocalizerService : ITextLocalizerService
    {
        #region Members

        public event EventHandler LocalizationChanged;

        #endregion

        #region Constructors

        public TextLocalizerService()
        {
        }

        #endregion

        #region Methods

        public void ChangeLanguage( string cultureName )
        {
            if ( string.IsNullOrEmpty( cultureName ) )
                throw new ArgumentNullException( nameof( cultureName ) );

            if ( cultureName == SelectedCulture?.Name )
                return;

            SelectedCulture = new CultureInfo( cultureName );

            CultureInfo.DefaultThreadCurrentCulture = SelectedCulture;
            CultureInfo.DefaultThreadCurrentUICulture = SelectedCulture;

            CultureInfo.CurrentCulture = SelectedCulture;
            CultureInfo.CurrentUICulture = SelectedCulture;

            LocalizationChanged?.Invoke( this, EventArgs.Empty );
        }

        #endregion

        #region Properties

        public CultureInfo SelectedCulture { get; private set; }

        #endregion
    }
}
