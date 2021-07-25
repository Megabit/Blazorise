using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Demo.Models;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Demo.Data
{
    public class EmployeeData
    {
        /// <summary>
        /// Simplified code to get data... ONLY for Demo purposes.
        /// You should never use this kind of way to get data, specially if it's volatile.
        /// </summary>
        /// <param name="httpClientfactory"></param>
        /// <param name="navigationManager"></param>
        public EmployeeData( IHttpClientFactory httpClientfactory, NavigationManager navigationManager )
        {
            using HttpClient client = httpClientfactory.CreateClient();
            client.BaseAddress = new( navigationManager.BaseUri );
            Data = client.GetFromJsonAsync<List<Employee>>( "_content/Blazorise.Demo/demoData.json" ).ConfigureAwait( false ).GetAwaiter().GetResult();
        }

        public List<Employee> Data { get; }

    }
}
