#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Component that allows you to display and edit multi-line text.
/// </summary>
public partial class MemoInput : BaseBufferedTextInput<string>, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// Captured ReplaceTab parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool> paramReplaceTab;

    /// <summary>
    /// Captured TabSize parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<int> paramTabSize;

    /// <summary>
    /// Captured SoftTabs parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool> paramSoftTabs;

    /// <summary>
    /// Captured AutoSize parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool> paramAutoSize;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void CaptureParameters( ParameterView parameters )
    {
        base.CaptureParameters( parameters );

        parameters.TryGetParameter( nameof( ReplaceTab ), ReplaceTab, out paramReplaceTab );
        parameters.TryGetParameter( nameof( TabSize ), TabSize, out paramTabSize );
        parameters.TryGetParameter( nameof( SoftTabs ), SoftTabs, out paramSoftTabs );
        parameters.TryGetParameter( nameof( AutoSize ), AutoSize, out paramAutoSize );
    }

    /// <inheritdoc/>
    protected override async Task OnBeforeSetParametersAsync( ParameterView parameters )
    {
        await base.OnBeforeSetParametersAsync( parameters );

        var replaceTabChanged = paramReplaceTab.Defined && paramReplaceTab.Changed;
        var tabSizeChanged = paramTabSize.Defined && paramTabSize.Changed;
        var softTabsChanged = paramSoftTabs.Defined && paramSoftTabs.Changed;
        var autoSizeChanged = paramAutoSize.Defined && paramAutoSize.Changed;

        if ( Rendered && ( replaceTabChanged
                           || tabSizeChanged
                           || softTabsChanged
                           || autoSizeChanged ) )
        {
            ExecuteAfterRender( async () => await JSModule.UpdateOptions( ElementRef, ElementId, new()
            {
                ReplaceTab = new JSOptionChange<bool>( replaceTabChanged, paramReplaceTab.Value ),
                TabSize = new JSOptionChange<int>( tabSizeChanged, paramTabSize.Value ),
                SoftTabs = new JSOptionChange<bool>( softTabsChanged, paramSoftTabs.Value ),
                AutoSize = new JSOptionChange<bool>( autoSizeChanged, paramAutoSize.Value ),
            } ) );

        }

        if ( Rendered )
        {
            if ( paramValue.Changed )
            {
                ExecuteAfterRender( async () =>
                {
                    await Revalidate();

                    if ( AutoSize )
                    {
                        await JSModule.RecalculateAutoHeight( ElementRef, ElementId );
                    }
                } );
            }
        }
    }

    /// <inheritdoc/>
    protected async override Task OnFirstAfterRenderAsync()
    {
        await JSModule.Initialize( ElementRef, ElementId, new()
        {
            ReplaceTab = ReplaceTab,
            TabSize = TabSize,
            SoftTabs = SoftTabs,
            AutoSize = AutoSize,
        } );


        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );
        }

        await base.DisposeAsync( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.MemoInput( Plaintext ) );
        builder.Append( ClassProvider.MemoInputSize( ThemeSize ) );
        builder.Append( ClassProvider.MemoInputValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<string>> ParseValueFromStringAsync( string value )
    {
        return Task.FromResult( new ParseValue<string>( true, value, null ) );
    }

    /// <inheritdoc/>
    protected override async Task OnInputHandler( ChangeEventArgs e )
    {
        if ( IsImmediate )
        {
            var value = e?.Value?.ToString();

            if ( IsDebounce )
            {
                InputValueDebouncer?.Update( value );
            }
            else
            {
                var caret = await JSUtilitiesModule.GetCaret( ElementRef );

                await CurrentValueHandler( value );

                await JSUtilitiesModule.SetCaret( ElementRef, caret );
            }
        }
    }

    /// <inheritdoc/>
    protected override Task OnKeyPressHandler( KeyboardEventArgs eventArgs )
    {
        if ( IsImmediate
             && IsDebounce
             && ( eventArgs?.Key?.Equals( "Enter", StringComparison.OrdinalIgnoreCase ) ?? false ) )
        {
            InputValueDebouncer?.Flush();
        }

        return base.OnKeyPressHandler( eventArgs );
    }

    /// <inheritdoc/>
    protected override Task OnBlurHandler( FocusEventArgs eventArgs )
    {
        if ( IsImmediate
             && IsDebounce )
        {
            InputValueDebouncer?.Flush();
        }

        return base.OnBlurHandler( eventArgs );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override string DefaultValue => string.Empty;

    /// <summary>
    /// Gets or sets the <see cref="IJSMemoInputModule"/> instance.
    /// </summary>
    [Inject] public IJSMemoInputModule JSModule { get; set; }

    /// <summary>
    /// Specifies the maximum number of characters allowed in the input element.
    /// </summary>
    [Parameter] public int? MaxLength { get; set; }

    /// <summary>
    /// Specifies the number lines in the input element.
    /// </summary>
    [Parameter] public int? Rows { get; set; }

    /// <summary>
    /// If set to true, <see cref="ReplaceTab"/> will insert a tab instead of cycle input focus.
    /// </summary>
    [Parameter] public bool ReplaceTab { get; set; } = false;

    /// <summary>
    /// Defines the number of characters that tab key will override.
    /// </summary>
    [Parameter] public int TabSize { get; set; } = 4;

    /// <summary>
    /// If set to true, spaces will be used instead of a tab character
    /// </summary>
    [Parameter] public bool SoftTabs { get; set; } = true;

    /// <summary>
    /// If true, the textarea will automatically grow in height according to its content.
    /// </summary>
    [Parameter] public bool AutoSize { get; set; }

    #endregion
}