// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Components;
using Blazorise.E2ETests.Infrastructure.ServerFixtures;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using Xunit.Abstractions;
using BasicTestApp.Server;

namespace Blazorise.E2ETests.Infrastructure
{
    public class BasicTestAppTestBase : ServerTestBase<ToggleExecutionModeServerFixture<Program>>
    {
        public string ServerPathBase
            => "/subdir" + ( _serverFixture.UsingAspNetHost ? "#server" : "" );

        public BasicTestAppTestBase(
            BrowserFixture browserFixture,
            ToggleExecutionModeServerFixture<Program> serverFixture,
            ITestOutputHelper output )
            : base( browserFixture, serverFixture, output )
        {
            serverFixture.PathBase = ServerPathBase;
        }

        protected IWebElement MountTestComponent<TComponent>() where TComponent : IComponent
        {
            var componentTypeName = typeof( TComponent ).FullName;
            var testSelector = WaitUntilTestSelectorReady();
            testSelector.SelectByValue( "none" );
            testSelector.SelectByValue( componentTypeName );
            return Browser.FindElement( By.TagName( "app" ) );
        }

        protected SelectElement WaitUntilTestSelectorReady()
        {
            var elemToFind = By.CssSelector( "#test-selector > select" );
            new WebDriverWait( Browser, TimeSpan.FromSeconds( 30 ) ).Until(
                driver => driver.FindElement( elemToFind ) != null );
            return new( Browser.FindElement( elemToFind ) );
        }
    }
}
