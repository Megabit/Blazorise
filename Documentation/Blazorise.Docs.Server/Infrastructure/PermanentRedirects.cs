using System.Collections.Generic;

namespace Blazorise.Docs.Server.Infrastructure;

static class PermanentRedirects
{
    public static readonly Dictionary<string, string> Map = new()
    {
        ["/docs/components/date"] = "/docs/components/date-input",
        ["/docs/components/file"] = "/docs/components/file-input",
        ["/docs/components/memo"] = "/docs/components/memo-input",
        ["/docs/components/numeric"] = "/docs/components/numeric-input",
        ["/docs/components/text"] = "/docs/components/text-input",
        ["/docs/components/time"] = "/docs/components/time-input",
        ["/docs/components/color"] = "/docs/components/color-input",
        ["/docs/components/dragdrop"] = "/docs/components/drag-drop",
    };
}
