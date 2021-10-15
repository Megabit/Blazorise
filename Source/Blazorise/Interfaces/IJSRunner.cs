#region Using directives
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface IJSRunner
    {
        #region Select

        ValueTask<TValue[]> GetSelectedOptions<TValue>( string elementId );

        ValueTask SetSelectedOptions<TValue>( string elementId, IReadOnlyList<TValue> values );

        #endregion
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
