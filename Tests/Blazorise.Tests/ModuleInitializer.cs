using System.Runtime.CompilerServices;
using VerifyTests;

namespace Blazorise.Tests
{
    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init()
        {
            VerifyBunit.Initialize();
            VerifierSettings.ScrubInlineGuids();
        }
    }
}