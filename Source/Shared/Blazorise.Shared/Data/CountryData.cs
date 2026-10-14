#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Blazorise.Shared.Models;
#endregion

namespace Blazorise.Shared.Data;

public class CountryData
{
    private static readonly Lazy<List<Country>> countries = new( LoadData );

    public Task<IEnumerable<Country>> GetDataAsync()
        => Task.FromResult<IEnumerable<Country>>( countries.Value );

    private static List<Country> LoadData()
    {
        Assembly assembly = typeof( EmployeeData ).Assembly;
        using var stream = assembly.GetManifestResourceStream( "Blazorise.Shared.Resources.CountryData.json" );
        return JsonSerializer.Deserialize<List<Country>>( new StreamReader( stream ).ReadToEnd() );
    }
}