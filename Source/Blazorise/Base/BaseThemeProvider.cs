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
    public abstract class BaseThemeProvider : ComponentBase
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

        protected virtual void GenerateStyle( StringBuilder sb )
        {
            if ( !string.IsNullOrEmpty( Theme?.Variants?.Primary ) )
                sb.AppendLine( $"--b-theme-variant-primary: {Theme.Variants.Primary};" );

            if ( !string.IsNullOrEmpty( Theme?.Variants?.Secondary ) )
                sb.AppendLine( $"--b-theme-variant-secondary: {Theme.Variants.Secondary};" );
        }

        public string GetStyleTag()
        {
            var sb = new StringBuilder();
            sb.AppendLine( "<style>" );
            sb.AppendLine( ":root" );
            sb.AppendLine( "{" );
            GenerateStyle( sb );
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

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
