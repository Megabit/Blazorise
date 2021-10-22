using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazorise.Demo.Models;
using CountryData;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;

namespace Blazorise.Demo.Data
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
            this.cache = memoryCache;
        }

        public Task<IEnumerable<Country>> GetDataAsync()
            => cache.GetOrCreateAsync( cacheKey, LoadData );

        private Task<IEnumerable<Country>> LoadData( ICacheEntry cacheEntry )
            => Task.FromResult(CountryLoader.CountryInfo.Take(100).Select(x=> new Country(x.Name, x.Iso, x.Capital) ));
            
    }

}