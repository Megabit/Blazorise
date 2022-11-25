using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Tests.Extensions;

internal static class TestExtensions
{
    public static TimeSpan WaitTime = TimeSpan.FromSeconds( 5 );

    public static void Click<T>( this IRenderedComponent<T> comp, string selector ) where T : IComponent
    {
        comp.WaitForElement( selector, WaitTime ).Click();
    }

    public static void Input<T, TInput>( this IRenderedComponent<T> comp, string selector, TInput value, Action<IElement> action = null ) where T : IComponent
    {
        var element = comp.WaitForElement( selector, WaitTime );
        if ( action is not null )
            action( element );
        element.Input( value );
    }
}