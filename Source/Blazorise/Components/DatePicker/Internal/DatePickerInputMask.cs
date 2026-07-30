#region Using directives
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.Vendors;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Coordinates the input mask applied to a date picker input.
/// </summary>
internal sealed class DatePickerInputMask
{
    #region Members

    private readonly IInputMaskDateTimeInputFormatConverter inputFormatConverter;

    private readonly IJSInputMaskModule jsModule;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new date picker input mask coordinator.
    /// </summary>
    /// <param name="inputFormatConverter">Converter used to translate .NET date formats to input mask formats.</param>
    /// <param name="jsModule">JavaScript module that manages the input mask.</param>
    public DatePickerInputMask( IInputMaskDateTimeInputFormatConverter inputFormatConverter, IJSInputMaskModule jsModule )
    {
        this.inputFormatConverter = inputFormatConverter;
        this.jsModule = jsModule;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Recreates the input mask using the supplied format.
    /// </summary>
    /// <param name="elementRef">Input element reference.</param>
    /// <param name="elementId">Input element identifier.</param>
    /// <param name="inputFormat">Date input format.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task RefreshAsync( ElementReference elementRef, string elementId, string inputFormat )
    {
        await DestroyAsync( elementRef, elementId );

        if ( string.IsNullOrWhiteSpace( inputFormat ) )
            return;

        await jsModule.Initialize( null, elementRef, elementId, new InputMaskJSOptions
        {
            Alias = "datetime",
            InputFormat = inputFormatConverter.Convert( inputFormat ),
            MaskPlaceholder = "_",
            ShowMaskOnFocus = true,
            ShowMaskOnHover = true,
            ClearMaskOnLostFocus = true,
            DispatchChangeOnComplete = true,
        } );

        IsInitialized = true;
    }

    /// <summary>
    /// Removes the input mask when it has been initialized.
    /// </summary>
    /// <param name="elementRef">Input element reference.</param>
    /// <param name="elementId">Input element identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DestroyAsync( ElementReference elementRef, string elementId )
    {
        if ( !IsInitialized )
            return;

        await jsModule.SafeDestroy( elementRef, elementId );
        IsInitialized = false;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets whether the input mask is currently initialized.
    /// </summary>
    public bool IsInitialized { get; private set; }

    #endregion
}