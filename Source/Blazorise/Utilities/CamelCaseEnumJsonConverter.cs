using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazorise.Utilities;

/// <summary>
/// JsonConverter for enums that converts to and from a Camel Case result
/// </summary>
public class CamelCaseEnumJsonConverter : JsonStringEnumConverter
{
    /// <inheritdoc />
    public CamelCaseEnumJsonConverter() : base( JsonNamingPolicy.CamelCase )
    {
    }
}