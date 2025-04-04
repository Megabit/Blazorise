using System.Collections.Generic;

namespace Blazorise.Docs.Server.Infrastructure;

static class PermanentRedirects
{
    public static readonly Dictionary<string, string> Map = new()
    {
        ["/docs/components/date"] = "/docs/components/date-edit",
        ["/docs/components/file"] = "/docs/components/file-edit",
        ["/docs/components/memo"] = "/docs/components/memo-edit",
        ["/docs/components/numeric"] = "/docs/components/numeric-edit",
        ["/docs/components/text"] = "/docs/components/text-edit",
        ["/docs/components/time"] = "/docs/components/time-edit",
    };
}
