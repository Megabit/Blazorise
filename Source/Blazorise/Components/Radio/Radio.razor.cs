#region Using directives
using System;
using System.Drawing;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Radio buttons allow the user to select one option from a set.
/// </summary>
/// <typeparam name="TValue">Radio option value type.</typeparam>
public partial class Radio<TValue> : BaseRadioComponent<TValue>, IDisposable
{
    #region Members

    /// <summary>
    /// Radio group name.
    /// </summary>
    private string group;

    /// <summary>
    /// Defines the color of a radio button(only when <see cref="RadioGroup{TValue}.Buttons"/> is true).
    /// </summary>
    private Color color;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if ( ParentRadioGroup is not null )
        {
            ParentRadioGroup.RadioCheckedChanged -= OnRadioChanged;
            ParentRadioGroup.RadioCheckedChanged += OnRadioChanged;

            // Parent group name have higher priority!
            if ( string.IsNullOrEmpty( Group ) )
            {
                Group = ParentRadioGroup.Name;
            }
        }

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Radio( AsButton ) );
        builder.Append( ClassProvider.RadioSize( AsButton, ThemeSize ) );
        builder.Append( ClassProvider.RadioCursor( Cursor ) );
        builder.Append( ClassProvider.RadioValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            if ( ParentRadioGroup is not null )
            {
                ParentRadioGroup.RadioCheckedChanged -= OnRadioChanged;
            }
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override Task OnChangeHandler( ChangeEventArgs eventArgs )
    {
        if ( ParentRadioGroup is not null )
            return ParentRadioGroup.NotifyRadioChanged( this );

        // Radio should always be inside of RadioGroup or otherwise it's "checked" state will not
        // be activated like it should be. I will leave this just in case that users want to use it
        // but I will need to state in the documentation that it's generally not supported.
        return CurrentValueHandler( eventArgs?.Value?.ToString() );
    }

    /// <summary>
    /// Event that raises after one of other radios inside of group changes.
    /// </summary>
    /// <param name="sender">Reference to the object that raised the event.</param>
    /// <param name="eventArgs">Information about the currently checked radio.</param>
    private async void OnRadioChanged( object sender, RadioCheckedChangedEventArgs<TValue> eventArgs )
    {
        // Some providers like AntDesign need additional changes on classes or styles.
        DirtyClasses();
        DirtyStyles();

        await InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool IsDisabled => ParentRadioGroup?.Disabled == true || base.IsDisabled;

    /// <summary>
    /// True if radio belongs to the <see cref="RadioGroup{TValue}"/>.
    /// </summary>
    protected bool ParentIsRadioGroup => ParentRadioGroup is not null;

    /// <summary>
    /// True if radio should look as a regular button.
    /// </summary>
    protected bool AsButton => ParentRadioGroup?.Buttons == true;

    /// <summary>
    /// Determines the color of the radio button.
    /// </summary>
    protected Color ButtonColor => Color ?? ParentRadioGroup?.Color ?? Color.Secondary;

    /// <summary>
    /// Determines if the radio button is in active state.
    /// </summary>
    protected bool IsActive => ParentRadioGroup is not null
        ? ParentRadioGroup.Value.IsEqual( Value )
        : CurrentValue.IsEqual( Value );

    /// <summary>
    /// Sets the radio group name.
    /// </summary>
    [Parameter]
    public string Group
    {
        get => group;
        set
        {
            group = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the color of a radio button(only when <see cref="RadioGroup{TValue}.Buttons"/> is true).
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
    /// Defines the intent of a radio button(only when <see cref="RadioGroup{TValue}.Buttons"/> is true).
    /// </summary>
    [Parameter]
    public Intent Intent
    {
        get => Color.ToIntent();
        set => Color = value.ToColor();
    }

    /// <summary>
    /// Radio group in which this radio is placed.
    /// </summary>
    [CascadingParameter] protected RadioGroup<TValue> ParentRadioGroup { get; set; }

    #endregion
}