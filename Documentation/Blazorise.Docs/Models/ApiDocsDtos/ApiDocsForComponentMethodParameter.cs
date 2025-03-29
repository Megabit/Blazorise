#region Using directives
#endregion

namespace Blazorise.Docs.Models.ApiDocsDtos;

public class ApiDocsForComponentMethodParameter
{
    public ApiDocsForComponentMethodParameter( string name, string typeName )
    {
        Name = name;
        TypeName = typeName;
    }

    public string Name { get; set; }

    public string TypeName { get; set; }
}