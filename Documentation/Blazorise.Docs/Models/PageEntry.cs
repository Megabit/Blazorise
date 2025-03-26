using Blazorise.Docs.Models.ApiDocsDtos;

namespace Blazorise.Docs.Models;

public class PageEntry( string url, string name, string description = null )
{
    public static PageEntry GetDocsPageEntryForParams(ApiDocsForComponent comp, IApiDocsRecord param) =>
        new(
        comp.SearchUrl,
        $"{comp.TypeName}.{param.Name}"
        );

    
    public string Url { get; } = url;
    public string Name { get; } = name;
    public string Description { get; } = description;
}