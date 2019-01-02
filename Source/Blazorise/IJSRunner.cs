#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
#endregion

namespace Blazorise
{
    public interface IJSRunner
    {
        Task<bool> Init( ElementRef elementRef, object componentRef );

        Task<bool> AddClass( ElementRef elementRef, string classname );

        Task<bool> RemoveClass( ElementRef elementRef, string classname );

        Task<bool> ToggleClass( ElementRef elementId, string classname );

        Task<bool> AddClassToBody( string classname );

        Task<bool> RemoveClassFromBody( string classname );

        Task<bool> ParentHasClass( ElementRef elementRef, string classaname );

        Task<string[]> GetFilePaths( ElementRef element );

        Task<bool> ActivateDatePicker( string elementId, string formatSubmit );

        Task<string[]> GetSelectedOptions( string elementId );
    }
}
