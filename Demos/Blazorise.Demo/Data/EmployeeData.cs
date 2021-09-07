using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Demo.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;

namespace Blazorise.Demo.Data
{
    public class EmployeeData
    {
        private readonly IHttpClientFactory httpClientfactory;
        private readonly NavigationManager navigationManager;
        private IMemoryCache cache;
        private string employeesCacheKey = "cache_employees";

        /// <summary>
        /// Simplified code to get & cache data in memory...
        /// </summary>
        /// <param name="httpClientfactory"></param>
        /// <param name="navigationManager"></param>
        public EmployeeData( IHttpClientFactory httpClientfactory, NavigationManager navigationManager, IMemoryCache memoryCache )
        {
            this.httpClientfactory = httpClientfactory;
            this.navigationManager = navigationManager;
            this.cache = memoryCache;
        }

        public Task<List<Employee>> GetDataAsync()
            => cache.GetOrCreateAsync<List<Employee>>( employeesCacheKey, LoadData );

        private async Task<List<Employee>> LoadData(ICacheEntry cacheEntry )
        {
            HttpClient httpClient = httpClientfactory.CreateClient();
            httpClient.BaseAddress = new( navigationManager.BaseUri );
            return await httpClient.GetFromJsonAsync<List<Employee>>( "_content/Blazorise.Demo/demoData.json" ).ConfigureAwait( false );
        }
    }

}