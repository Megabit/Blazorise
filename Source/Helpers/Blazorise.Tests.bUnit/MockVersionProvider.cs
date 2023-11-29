#region Using directives
#endregion

namespace Blazorise.Tests.bUnit;


internal class MockVersionProvider : IVersionProvider
{
    public string Version => "";

    public string MilestoneVersion => "";
}
