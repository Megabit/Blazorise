using System.Collections.Generic;

namespace Blazorise.Docs.Server.Infrastructure;

static class PermanentRedirects
{
    public static readonly Dictionary<string, string> Map = new()
    {
        ["/docs/components/date"] = "/docs/components/dateedit",
        ["/docs/components/file"] = "/docs/components/fileedit",
        ["/docs/components/memo"] = "/docs/components/memoedit",
        ["/docs/components/numeric"] = "/docs/components/numericedit",
        ["/docs/components/text"] = "/docs/components/textedit",
        ["/docs/components/time"] = "/docs/components/timeedit",
    };
}
