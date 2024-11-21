#region Using directives

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Blazorise.Shared.Models;
using Microsoft.Extensions.Caching.Memory;

#endregion

namespace Blazorise.Shared.Data;

public class Gender
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
}

public class EmployeeData
{
    private readonly IMemoryCache cache;
    private readonly string employeesCacheKey = "cache_employees";

    /// <summary>
    /// Simplified code to get & cache data in memory...
    /// </summary>
    public EmployeeData( IMemoryCache memoryCache )
    {
        cache = memoryCache;
    }

    public static IEnumerable<Gender> Genders = new List<Gender>()
    {
        new()
        {
            Id = 1 ,
            Code = null,
            Description = string.Empty
        },
        new()
        {
            Id = 2 ,
            Code = "M",
            Description = "Male"
        },
        new()
        {
            Id = 3 ,
            Code = "F",
            Description = "Female"
        },
        new()
        {
            Id = 4 ,
            Code = "D",
            Description = "Diverse"
        }
    };

    public async Task<List<Employee>> GetDataAsync()
        => ( await cache.GetOrCreateAsync( employeesCacheKey, LoadData ) )
        .Select( x => new Employee( x ) ) //new() is used so we make sure that we are not returning the same item references avoiding an application wide "data corruption".
        .ToList();

    private Task<List<Employee>> LoadData( ICacheEntry cacheEntry )
    {
        Assembly assembly = typeof( EmployeeData ).Assembly;
        using var stream = assembly.GetManifestResourceStream( "Blazorise.Shared.Resources.EmployeeData.json" );
        return Task.FromResult( JsonSerializer.Deserialize<List<Employee>>( new StreamReader( stream ).ReadToEnd() ) );
    }
}