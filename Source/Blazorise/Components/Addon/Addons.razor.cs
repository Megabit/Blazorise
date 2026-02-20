#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Wrapper for text, buttons, or button groups on either side of textual inputs.
/// </summary>
public partial class Addons : BaseComponent, IDisposable
{
    #region Members

    private Size? size;

    private List<Button> registeredButtons;

    private Validation previousParentValidation;

    private ValidationStatus previousValidationStatus;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        if ( ParentValidation != previousParentValidation )
        {
            DetachValidationStatusChangedListener();

            if ( ParentValidation is not null )
                ParentValidation.ValidationStatusChanged += OnValidationStatusChanged;

            previousParentValidation = ParentValidation;
            previousValidationStatus = ParentValidation?.Status ?? ValidationStatus.None;

            DirtyClasses();
        }
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        previousValidationStatus = ParentValidation?.Status ?? ValidationStatus.None;

        await base.OnInitializedAsync();

        if ( Theme is not null )
        {
            Theme.Changed += OnThemeChanged;
        }
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender && registeredButtons?.Count > 0 )
        {
            DirtyClasses();

            await InvokeAsync( StateHasChanged );
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            DetachValidationStatusChangedListener();

            if ( ParentValidation is not null )
            {
                ParentValidation.ValidationStatusChanged -= OnValidationStatusChanged;
            }

            if ( Theme is not null )
            {
                Theme.Changed -= OnThemeChanged;
            }
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Addons() );
        builder.Append( ClassProvider.AddonsSize( ThemeSize ) );
        builder.Append( ClassProvider.AddonsHasButton( registeredButtons?.Count > 0 ) );
        builder.Append( ClassProvider.AddonsValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation is not null );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Unsubscribe from <see cref="Validation.StatusChanged"/> event.
    /// </summary>
    private void DetachValidationStatusChangedListener()
    {
        if ( previousParentValidation is not null )
        {
            previousParentValidation.ValidationStatusChanged -= OnValidationStatusChanged;
        }
    }

    /// <summary>
    /// Handles the <see cref="Validation.StatusChanged"/> event.
    /// </summary>
    /// <param name="sender">Object that raised the event.</param>
    /// <param name="eventArgs">Data about the <see cref="Validation"/> status change event.</param>
    protected void OnValidationStatusChanged( object sender, ValidationStatusChangedEventArgs eventArgs )
    {
        if ( previousValidationStatus != eventArgs.Status )
        {
            previousValidationStatus = eventArgs.Status;

            DirtyClasses();

            InvokeAsync( StateHasChanged );
        }
    }

    /// <summary>
    /// Notify addons that a button is placed inside of it.
    /// </summary>
    /// <param name="button">A button reference that is placed inside of the addons.</param>
    internal void NotifyButtonInitialized( Button button )
    {
        if ( button is null )
            return;

        registeredButtons ??= new();

        if ( !registeredButtons.Contains( button ) )
        {
            registeredButtons.Add( button );
        }
    }

    /// <summary>
    /// Notify addons that a button is removed from it.
    /// </summary>
    /// <param name="button">A button reference that is placed inside of the addons.</param>
    internal void NotifyButtonRemoved( Button button )
    {
        if ( button is null )
            return;

        if ( registeredButtons is not null && registeredButtons.Contains( button ) )
        {
            registeredButtons.Remove( button );
        }
    }

    /// <summary>
    /// An event raised when theme settings changes.
    /// </summary>
    /// <param name="sender">An object that raised the event.</param>
    /// <param name="eventArgs"></param>
    private void OnThemeChanged( object sender, EventArgs eventArgs )
    {
        DirtyClasses();
        DirtyStyles();

        InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// True if <see cref="Addons"/> is placed inside of <see cref="Field"/> component.
    /// </summary>
    protected virtual bool ParentIsHorizontal => ParentField?.Horizontal == true;

    /// <summary>
    /// Gets the size based on the theme settings.
    /// </summary>
    protected Size ThemeSize => Size.GetValueOrDefault( Theme?.InputOptions?.Size ?? Blazorise.Size.Default );

    /// <summary>
    /// Changes the size of the elements placed inside of this <see cref="Addons"/>.
    /// </summary>
    [Parameter]
    public Size? Size
    {
        get => size;
        set
        {
            size = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Addons"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the reference to the parent <see cref="Field"/> component.
    /// </summary>
    [CascadingParameter] protected Field ParentField { get; set; }

    /// <summary>
    /// Cascaded theme settings.
    /// </summary>
    [CascadingParameter] public Theme Theme { get; set; }

    /// <summary>
    /// A reference to the parent <see cref="Validation"/> component in which this component is nested.
    /// </summary>
    [CascadingParameter] protected Validation ParentValidation { get; set; }

    #endregion
}