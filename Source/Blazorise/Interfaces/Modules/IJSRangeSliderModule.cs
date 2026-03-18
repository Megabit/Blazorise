#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Contracts for the <see cref="RangeSlider{TValue}"/> JS module.
/// </summary>
public interface IJSRangeSliderModule : IBaseJSModule,
    IJSDestroyableModule
{
    /// <summary>
    /// Initializes the new <see cref="RangeSlider{TValue}"/> within the JS module.
    /// </summary>
    /// <param name="startElementRef">Reference to the rendered start input element.</param>
    /// <param name="startElementId">ID of the rendered start input element.</param>
    /// <param name="endElementRef">Reference to the rendered end input element.</param>
    /// <param name="endElementId">ID of the rendered end input element.</param>
    /// <param name="clampToOtherHandle">True if the active handle should stop at the other handle.</param>
    /// <param name="allowEqualValues">True if handles are allowed to have the same value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize( ElementReference startElementRef, string startElementId, ElementReference endElementRef, string endElementId, bool clampToOtherHandle, bool allowEqualValues );
}