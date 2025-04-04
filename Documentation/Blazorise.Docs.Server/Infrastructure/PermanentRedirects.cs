using System.Collections.Generic;

namespace Blazorise.Docs.Server.Infrastructure;

static class PermanentRedirects
{
    public static readonly Dictionary<string, string> Map = new()
    {
        ["/docs/components/file"] = "/docs/components/fileedit",
        ["/docs/components/text"] = "/docs/components/textedit",
    };
}
