#region Using directives
using System;
using System.Text;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Main theme provider that will build the CSS variables and styles.
/// </summary>
public partial class ThemeProvider : ComponentBase, IDisposable
{
    #region Members

    private Theme theme;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public void Dispose()
    {
        if ( theme is not null )
        {
            theme.Changed -= OnOptionsChanged;
        }
    }

    /// <summary>
    /// Gets the html tag that will be inserted into the DOM and that will contain the variables.
    /// </summary>
    /// <returns>HTML variables tag element content.</returns>
    public string GetVariablesTag()
    {
        if ( !theme?.Enabled == true )
            return null;

        var sb = new StringBuilder();

        sb.AppendLine( "<style id='b-theme-variables'>" );
        sb.AppendLine( ":root" );
        sb.AppendLine( "{" );

        sb.AppendLine( ThemeGenerator.GenerateVariables( Theme ) );

        sb.AppendLine( "}" );
        sb.AppendLine( "</style>" );

        return sb.ToString();
    }

    /// <summary>
    /// Gets the html tag that will be inserted into the DOM and that will contain the styles.
    /// </summary>
    /// <returns>HTML variables tag element content.</returns>
    public string GetStylesTag()
    {
        if ( !theme?.Enabled == true )
            return null;

        var sb = new StringBuilder();

        sb.AppendLine( "<style id='b-theme-styles' type=\"text/css\" scoped>" );

        sb.AppendLine( ThemeGenerator.GenerateStyles( Theme ) );

        sb.AppendLine( "</style>" );

        return sb.ToString();
    }

    private async void OnOptionsChanged( object sender, EventArgs e )
    {
        await InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the theme options.
    /// </summary>
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

            if ( theme is not null )
            {
                theme.Changed -= OnOptionsChanged;
            }

            theme = value;

            if ( theme is not null )
            {
                theme.Changed += OnOptionsChanged;
            }
        }
    }

    /// <summary>
    /// If true variables will be written to the page body.
    /// </summary>
    [Parameter] public bool WriteVariables { get; set; } = true;

    /// <summary>
    /// Gets or sets the <see cref="IThemeGenerator">Theme Generator</see> used to build the CSS variables and styles.
    /// </summary>
    [Inject] protected IThemeGenerator ThemeGenerator { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="ThemeProvider"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}