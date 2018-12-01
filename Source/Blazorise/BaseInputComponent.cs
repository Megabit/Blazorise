#region Using directives
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise
{
    public class BaseInputComponent : BaseComponent
    {
        /// <summary>
        /// Sets the size of the input control.
        /// </summary>
        [Parameter] protected Size Size { get; set; } = Size.None;

        /// <summary>
        /// Add the readonly boolean attribute on an input to prevent modification of the input’s value.
        /// </summary>
        [Parameter] protected bool IsReadonly { get; set; }

        /// <summary>
        /// Add the disabled boolean attribute on an input to prevent user interactions and make it appear lighter.
        /// </summary>
        [Parameter] protected bool IsDisabled { get; set; }
    }
}
