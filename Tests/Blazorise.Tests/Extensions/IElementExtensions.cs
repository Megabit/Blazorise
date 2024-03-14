using System.Threading.Tasks;
using AngleSharp.Dom;
using Blazorise.Tests.Extensions;
using Bunit;

namespace Blazorise.Tests.Extensions;

public static class IElementExtensions
{
    public static Task ClickAsync( this IElement element )
        => element.ClickAsync( new Microsoft.AspNetCore.Components.Web.MouseEventArgs() { Detail = 1 } );

    public static Task InputAsync( this IElement element, string value )
        => element.InputAsync( new Microsoft.AspNetCore.Components.ChangeEventArgs() { Value = value } );

    public static Task ChangeAsync( this IElement element, string value )
        => element.ChangeAsync( new Microsoft.AspNetCore.Components.ChangeEventArgs() { Value = value } );
}
