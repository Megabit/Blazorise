using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Tests.Extensions
{
    internal static class TestExtensions
    {
        private static TimeSpan waitTime = TimeSpan.FromSeconds( 3 );

        public static void WaitForElementAndClick<T>( this IRenderedComponent<T> comp, string selector ) where T : IComponent
        {
            comp.WaitForElement( selector, waitTime ).Click();
        }

        public static void WaitForElementAndInput<T, TInput>( this IRenderedComponent<T> comp, string selector, TInput value, Action<IElement> action = null ) where T : IComponent
        {
            var element = comp.WaitForElement( selector, waitTime );
            if ( action is not null )
                action( element );
            element.Input( value );
        }
    }
}
