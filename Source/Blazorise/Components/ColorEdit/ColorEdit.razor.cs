#region Using directives
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// The editor that allows you to select a color from a dropdown menu.
/// </summary>
public partial class ColorEdit : BaseInputComponent<string>, ISelectableComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ColorEdit() );
        builder.Append( ClassProvider.ColorEditSize( ThemeSize ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Handles the input onchange event.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnChangeHandler( ChangeEventArgs eventArgs )
    {
        return CurrentValueHandler( eventArgs?.Value?.ToString() );
    }

    /// <inheritdoc/>
    protected override string FormatValueAsString( string value )
    {
        return value;
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<string>> ParseValueFromStringAsync( string value )
    {
        return Task.FromResult( new ParseValue<string>( true, value, null ) );
    }

    /// <inheritdoc/>
    public virtual Task Select( bool focus = true )
    {
        return JSUtilitiesModule.Select( ElementRef, ElementId, focus ).AsTask();
    }

    #endregion
}