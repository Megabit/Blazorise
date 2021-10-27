#region Using directives
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Blazorise.Shared.Models;
using Microsoft.Extensions.Caching.Memory;
#endregion

namespace Blazorise.Shared.Data
{
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

        public Task<List<Employee>> GetDataAsync()
            => cache.GetOrCreateAsync( employeesCacheKey, LoadData );

        private Task<List<Employee>> LoadData( ICacheEntry cacheEntry )
        {
            Assembly assembly = typeof( EmployeeData ).Assembly;
            using var stream = assembly.GetManifestResourceStream( "Blazorise.Shared.Resources.EmployeeData.json" );
            return Task.FromResult( JsonSerializer.Deserialize<List<Employee>>( new StreamReader( stream ).ReadToEnd() ) );
        }
    }
}