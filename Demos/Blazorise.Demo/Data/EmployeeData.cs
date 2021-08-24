using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Demo.Models;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Demo.Data
{
    public class EmployeeData
    {
        private readonly IHttpClientFactory httpClientfactory;
        private readonly HttpClient httpClient;
        private readonly NavigationManager navigationManager;

        /// <summary>
        /// Simplified code to get & cache data in memory...
        /// </summary>
        /// <param name="httpClientfactory"></param>
        /// <param name="navigationManager"></param>
        public EmployeeData( IHttpClientFactory httpClientfactory, NavigationManager navigationManager )
        {
            this.httpClientfactory = httpClientfactory;
            this.navigationManager = navigationManager;
            httpClient = httpClientfactory.CreateClient();
            httpClient.BaseAddress = new( navigationManager.BaseUri );
        }

        private List<Employee> data;

        public async Task<List<Employee>> GetDataAsync()
        {
            if ( data is null )
                await LoadData().ConfigureAwait( false );
            return data;
        }

        private async Task LoadData()
        {
            data = await httpClient.GetFromJsonAsync<List<Employee>>( "_content/Blazorise.Demo/demoData.json" ).ConfigureAwait( false );
        }
    }

}