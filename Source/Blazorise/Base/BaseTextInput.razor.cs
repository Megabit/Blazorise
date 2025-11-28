#region Using directives
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Base component for inputs that are text-based.
/// </summary>
/// <typeparam name="TValue">Editable value type.</typeparam>
public abstract class BaseTextInput<TValue> : BaseInputComponent<TValue>, ISelectableComponent, IDisposable
{
    #region Members

    private Color color = Color.Default;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override async Task OnAfterSetParametersAsync( ParameterView parameters )
    {
        if ( ParentValidation is not null )
        {
            if ( parameters.TryGetValue<string>( nameof( Pattern ), out var paramPattern ) )
            {
                // make sure we get the newest value
                var newValue = paramValue.Defined
                    ? paramValue.Value
                    : Value;

                await ParentValidation.InitializeInputPattern( paramPattern, newValue );
            }
        }

        await base.OnAfterSetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ReleaseResources();
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            ReleaseResources();
        }

        return base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Shared code to dispose of any internal resources.
    /// </summary>
    protected override void ReleaseResources()
    {
        base.ReleaseResources();
    }

    /// <summary>
    /// Handler for @onchange event.
    /// </summary>
    /// <param name="eventArgs">Information about the changed event.</param>
    /// <returns>Returns awaitable task</returns>
    protected virtual Task OnChangeHandler( ChangeEventArgs eventArgs )
        => CurrentValueHandler( eventArgs?.Value?.ToString() );

    /// <inheritdoc/>
    public virtual Task Select( bool focus = true )
    {
        return JSUtilitiesModule.Select( ElementRef, ElementId, focus ).AsTask();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Sets the placeholder for the empty text.
    /// </summary>
    [Parameter] public string Placeholder { get; set; }

    /// <summary>
    /// Sets the class to remove the default form field styling and preserve the correct margin and padding.
    /// </summary>
    [Parameter] public bool Plaintext { get; set; }

    /// <summary>
    /// Sets the input text color.
    /// </summary>
    [Parameter]
    public Color Color
    {
        get => color;
        set
        {
            color = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// The pattern attribute specifies a regular expression that the input element's value is checked against on form validation.
    /// </summary>
    [Parameter] public string Pattern { get; set; }

    #endregion
}