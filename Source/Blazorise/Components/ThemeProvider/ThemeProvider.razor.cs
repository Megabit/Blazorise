#region Using directives
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Main theme provider that will build the CSS variables and styles.
/// </summary>
public partial class ThemeProvider : ComponentBase, IDisposable
{
    #region Members

    private Theme subscribedTheme;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        var effectiveTheme = parameters.TryGetValue<Theme>( nameof( Theme ), out var paramTheme ) ? paramTheme : Theme;
        var cascadedTheme = parameters.TryGetValue<Theme>( nameof( CascadedTheme ), out var paramCascadedTheme ) ? paramCascadedTheme : CascadedTheme;

        effectiveTheme ??= cascadedTheme;

        if ( !ReferenceEquals( subscribedTheme, effectiveTheme ) )
        {
            if ( subscribedTheme is not null )
            {
                subscribedTheme.Changed -= OnOptionsChanged;
            }

            subscribedTheme = effectiveTheme;

            if ( subscribedTheme is not null )
            {
                subscribedTheme.Changed += OnOptionsChanged;
            }
        }

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if ( subscribedTheme is not null )
        {
            subscribedTheme.Changed -= OnOptionsChanged;
        }
    }

    /// <summary>
    /// Gets the html tag that will be inserted into the DOM and that will contain the variables.
    /// </summary>
    /// <returns>HTML variables tag element content.</returns>
    public string GetVariablesTag()
    {
        var effectiveTheme = EffectiveTheme;

        if ( effectiveTheme?.Enabled != true )
        {
            return null;
        }

        var sb = new StringBuilder();

        sb.AppendLine( "<style id='b-theme-variables'>" );
        sb.AppendLine( ":root" );
        sb.AppendLine( "{" );

        sb.AppendLine( ThemeGenerator.GenerateVariables( effectiveTheme ) );

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
        var effectiveTheme = EffectiveTheme;

        if ( effectiveTheme?.Enabled != true )
        {
            return null;
        }

        var sb = new StringBuilder();

        sb.AppendLine( "<style id='b-theme-styles' type=\"text/css\" scoped>" );

        sb.AppendLine( ThemeGenerator.GenerateStyles( effectiveTheme ) );

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
    /// Gets the explicit theme or the theme supplied by an ancestor or root-level registration.
    /// </summary>
    protected Theme EffectiveTheme => Theme ?? CascadedTheme;

    /// <summary>
    /// Defines the <see cref="IThemeGenerator">Theme Generator</see> used to build the CSS variables and styles.
    /// </summary>
    [Inject] protected IThemeGenerator ThemeGenerator { get; set; }

    /// <summary>
    /// Specifies the theme options. When omitted or null, uses the cascading theme.
    /// </summary>
    [Parameter] public Theme Theme { get; set; }

    /// <summary>
    /// If true variables will be written to the page body.
    /// </summary>
    [Parameter] public bool WriteVariables { get; set; } = true;

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="ThemeProvider"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the theme supplied by an ancestor or root-level registration.
    /// </summary>
    [CascadingParameter] protected Theme CascadedTheme { get; set; }

    #endregion
}