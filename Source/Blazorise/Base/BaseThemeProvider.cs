﻿#region Using directives
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

        public string GetVariablesTag()
        {
            var sb = new StringBuilder();

            sb.AppendLine( "<style>" );
            sb.AppendLine( ":root" );
            sb.AppendLine( "{" );

            ThemeGenerator.GenerateVariables( sb, Theme );

            sb.AppendLine( "}" );
            sb.AppendLine( "</style>" );

            return sb.ToString();
        }

        public string GetStylesTag()
        {
            var sb = new StringBuilder();

            sb.AppendLine( $"<style type=\"text/css\" scoped>" );

            ThemeGenerator.GenerateStyles( sb, Theme );

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

        /// <summary>
        /// If true variables will be written to the page body.
        /// </summary>
        [Parameter] protected bool WriteVariables { get; set; } = true;

        [Inject] protected IThemeGenerator ThemeGenerator { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
