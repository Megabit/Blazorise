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

namespace Blazorise.Shared.Data
{
    public class CountryData
    {
        private readonly IMemoryCache cache;
        private readonly string cacheKey = "cache_countries";

        /// <summary>
        /// Simplified code to get & cache data in memory...
        /// </summary>
        public CountryData( IMemoryCache memoryCache )
        {
            cache = memoryCache;
        }

        public Task<IEnumerable<Country>> GetDataAsync()
            => cache.GetOrCreateAsync( cacheKey, LoadData );

        private Task<IEnumerable<Country>> LoadData( ICacheEntry cacheEntry )
        {
            Assembly assembly = typeof( EmployeeData ).Assembly;
            using var stream = assembly.GetManifestResourceStream( "Blazorise.Shared.Resources.CountryData.json" );
            return Task.FromResult( JsonSerializer.Deserialize<List<Country>>( new StreamReader( stream ).ReadToEnd() ).AsEnumerable() );
        }
    }
}