//// Copyright (c) .NET Foundation. All rights reserved.
//// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

//using Blazorise.BasicTestApp;
//using Microsoft.AspNetCore.Components.Browser.Rendering;
//using Blazorise.UnitTests.Infrastructure;
//using Blazorise.UnitTests.Infrastructure.ServerFixtures;
//using Blazorise.UnitTests.Tests;
//using OpenQA.Selenium;
//using System;
//using System.Threading.Tasks;
//using Xunit;
//using Xunit.Abstractions;

//namespace Blazorise.UnitTests.ServerExecutionTests
//{
//    // By inheriting from ComponentRenderingTest, this test class also copies
//    // all the test cases shared with client-side rendering

//    public class ServerComponentRenderingTest : ComponentRenderingTest
//    {
//        public ServerComponentRenderingTest(BrowserFixture browserFixture, ToggleExecutionModeServerFixture<Program> serverFixture, ITestOutputHelper output)
//            : base(browserFixture, serverFixture.WithServerExecution(), output)
//        {
//        }

//        [Fact]
//        public void ThrowsIfRenderIsRequestedOutsideSyncContext()
//        {
//            var appElement = MountTestComponent<DispatchingComponent>();
//            var result = appElement.FindElement(By.Id("result"));

//            appElement.FindElement(By.Id("run-without-dispatch")).Click();

//            WaitAssert.Contains(
//                $"{typeof(InvalidOperationException).FullName}: The current thread is not associated with the renderer's synchronization context",
//                () => result.Text);
//        }
//    }
//}
