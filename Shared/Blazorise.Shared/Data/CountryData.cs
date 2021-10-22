using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Shared.Models;
using CountryData;
using Microsoft.Extensions.Caching.Memory;

namespace Blazorise.Shared.Data
{
    public class CountryData
    {
        private IMemoryCache cache;
        private string cacheKey = "cache_countries";

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
            => Task.FromResult( CountryLoader.CountryInfo.Take( 100 ).Select( x => new Country( x.Name, x.Iso, x.Capital ) ) );

    }

}