// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using OpenQA.Selenium;
using Xunit;
using Xunit.Abstractions;

namespace Blazorise.E2ETests.Infrastructure
{
    [CaptureSeleniumLogs]
    public class BrowserTestBase : IClassFixture<BrowserFixture>
    {
        private static readonly AsyncLocal<IWebDriver> _browser = new();
        private static readonly AsyncLocal<ILogs> _logs = new();
        private static readonly AsyncLocal<ITestOutputHelper> _output = new();

        public static IWebDriver Browser => _browser.Value;

        public static ILogs Logs => _logs.Value;

        public static ITestOutputHelper Output => _output.Value;

        public BrowserTestBase(BrowserFixture browserFixture, ITestOutputHelper output)
        {
            _browser.Value = browserFixture.Browser;
            _logs.Value = browserFixture.Logs;
            _output.Value = output;
        }
    }
}
