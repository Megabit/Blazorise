using System.Threading.Tasks;
using AngleSharp.Dom;
using Blazorise.Tests.Extensions;
using Bunit;

namespace Blazorise.Tests.Extensions;

public static class IElementExtensions
{
    public static Task InputAsync( this IElement element, string value )
        => element.InputAsync( new Microsoft.AspNetCore.Components.ChangeEventArgs() { Value = value } );
}
