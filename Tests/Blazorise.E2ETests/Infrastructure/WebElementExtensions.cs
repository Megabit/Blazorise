using OpenQA.Selenium;

namespace Blazorise.E2ETests.Infrastructure
{
    public static class WebElementExtensions
    {
        public static bool ElementIsPresent( this IWebElement element, By by )
        {
            try
            {
                return element.FindElement( by ).Displayed;
            }
            catch ( NoSuchElementException )
            {
                return false;
            }
        }

        public static void SendKeysSequentially( this IWebElement target, string text )
        {
            // Calling it for each character works around some chars being skipped
            // https://stackoverflow.com/a/40986041
            foreach ( var c in text )
            {
                target.SendKeys( c.ToString() );
            }
        }

        public static void ClearText( this IWebElement target )
        {
            int length = target.Text.Length;

            for ( int i = length; i > 0; --i )
            {
                target.SendKeys( Keys.Backspace );
            }
        }
    }
}
