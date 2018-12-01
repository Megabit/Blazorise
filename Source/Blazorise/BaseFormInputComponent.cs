#region Using directives
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise
{
    public class BaseFormInputComponent : BaseInputComponent
    {
        /// <summary>
        /// Gets or sets the custom css classname(s) of the input control inside the form group.
        /// </summary>
        [Parameter] protected string InnerClass { get; set; }
    }
}
