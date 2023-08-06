#region Using directives
using System.Text.Json;
using System.Text.Json.Serialization; 
#endregion

namespace Blazorise.Utilities.JsonConverters;

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