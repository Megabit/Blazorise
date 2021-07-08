using System.Globalization;
using System.Runtime.CompilerServices;
using Verify.AngleSharp;
using VerifyTests;

namespace Blazorise.Tests
{
    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            VerifyBunit.Initialize();
            HtmlPrettyPrint.All();
            VerifierSettings.ScrubInlineGuids();
        }
    }
}