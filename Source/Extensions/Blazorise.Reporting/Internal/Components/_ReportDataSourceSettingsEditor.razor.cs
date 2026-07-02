#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Internal fallback editor for provider settings without a dedicated editor component.
/// </summary>
public partial class _ReportDataSourceSettingsEditor
{
    #region Members

    private readonly List<ReportDataSourceSettingItem> settings = [];

    private ReportDataSourceProviderEditorContext previousContext;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if ( ReferenceEquals( previousContext, Context ) )
            return;

        previousContext = Context;
        settings.Clear();
        settings.AddRange( Context?.Settings?.Select( setting => new ReportDataSourceSettingItem
        {
            Key = setting.Key,
            Value = Convert.ToString( setting.Value, CultureInfo.InvariantCulture ),
        } ) ?? [] );
    }

    private Task AddSettingAsync()
    {
        settings.Add( new() );
        UpdateContextSettings();

        return Task.CompletedTask;
    }

    private Task RemoveSettingAsync( ReportDataSourceSettingItem setting )
    {
        settings.Remove( setting );
        UpdateContextSettings();

        return Task.CompletedTask;
    }

    private Task OnSettingKeyChanged( ReportDataSourceSettingItem setting, string value )
    {
        setting.Key = value;
        UpdateContextSettings();

        return Task.CompletedTask;
    }

    private Task OnSettingValueChanged( ReportDataSourceSettingItem setting, string value )
    {
        setting.Value = value;
        UpdateContextSettings();

        return Task.CompletedTask;
    }

    private void UpdateContextSettings()
    {
        if ( Context is null )
            return;

        Context.Settings.Clear();

        foreach ( ReportDataSourceSettingItem setting in settings )
        {
            if ( string.IsNullOrWhiteSpace( setting.Key ) )
                continue;

            Context.Settings[setting.Key.Trim()] = setting.Value;
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Provider settings context edited by the fallback editor.
    /// </summary>
    [Parameter] public ReportDataSourceProviderEditorContext Context { get; set; }

    #endregion
}