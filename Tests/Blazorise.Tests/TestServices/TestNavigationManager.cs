using System;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Tests.TestServices
{
    public class TestNavigationManager : NavigationManager
    {
        public TestNavigationManager() => Initialize( "https://www.example.com/", "https://www.example.com/base" );

        protected override void NavigateToCore( string uri, bool forceLoad )
        {
            throw new NotImplementedException();
        }
    }
}