#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseThemeProvider : ComponentBase, IDisposable
    {
        #region Members

        private Theme theme;

        #endregion

        #region Methods

        public void Dispose()
        {
            if ( theme != null )
            {
                theme.Changed -= OnOptionsChanged;
            }
        }

        protected virtual void GenerateVariables( StringBuilder sb )
        {
            #region Variants

            if ( !string.IsNullOrEmpty( Theme?.Variants?.Primary ) )
                sb.AppendLine( $"--b-theme-variant-primary: {Theme.Variants.Primary};" );

            if ( !string.IsNullOrEmpty( Theme?.Variants?.Secondary ) )
                sb.AppendLine( $"--b-theme-variant-secondary: {Theme.Variants.Secondary};" );

            if ( !string.IsNullOrEmpty( Theme?.Variants?.Success ) )
                sb.AppendLine( $"--b-theme-variant-success: {Theme.Variants.Success};" );

            if ( !string.IsNullOrEmpty( Theme?.Variants?.Info ) )
                sb.AppendLine( $"--b-theme-variant-info: {Theme.Variants.Info};" );

            if ( !string.IsNullOrEmpty( Theme?.Variants?.Warning ) )
                sb.AppendLine( $"--b-theme-variant-warning: {Theme.Variants.Warning};" );

            if ( !string.IsNullOrEmpty( Theme?.Variants?.Danger ) )
                sb.AppendLine( $"--b-theme-variant-danger: {Theme.Variants.Danger};" );

            if ( !string.IsNullOrEmpty( Theme?.Variants?.Light ) )
                sb.AppendLine( $"--b-theme-variant-light: {Theme.Variants.Light};" );

            if ( !string.IsNullOrEmpty( Theme?.Variants?.Dark ) )
                sb.AppendLine( $"--b-theme-variant-dark: {Theme.Variants.Dark};" );

            #endregion

            #region Buttons

            if ( !string.IsNullOrEmpty( Theme?.ButtonFocusWidth ) )
                sb.AppendLine( $"--b-theme-button-focus-width: {Theme.ButtonFocusWidth};" );

            #endregion
        }

        public string GetVariablesTag()
        {
            var sb = new StringBuilder();

            sb.AppendLine( "<style>" );
            sb.AppendLine( ":root" );
            sb.AppendLine( "{" );

            GenerateVariables( sb );

            sb.AppendLine( "}" );
            sb.AppendLine( "</style>" );

            return sb.ToString();
        }

        private void OnOptionsChanged( object sender, EventArgs e )
        {
            StateHasChanged();
        }

        #endregion

        #region Properties

        [Parameter]
        public Theme Theme
        {
            get => theme;
            set
            {
                if ( theme == value )
                {
                    return;
                }

                if ( theme != null )
                {
                    theme.Changed -= OnOptionsChanged;
                }

                theme = value;

                if ( theme != null )
                {
                    theme.Changed += OnOptionsChanged;
                }
            }
        }

        [Inject] protected IClassProvider ClassProvider { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
