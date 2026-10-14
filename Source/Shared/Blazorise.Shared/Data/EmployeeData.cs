#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Blazorise.Shared.Models;
#endregion

namespace Blazorise.Shared.Data;

public class Gender
{
    public string Code { get; set; }
    public string Description { get; set; }
}

public class EmployeeData
{
    private static readonly Lazy<List<Employee>> employees = new( LoadData );

    public static IEnumerable<Gender> Genders = new List<Gender>()
    {
        new()
        {
            Code = null,
            Description = string.Empty
        },
        new()
        {
            Code = "M",
            Description = "Male"
        },
        new()
        {
            Code = "F",
            Description = "Female"
        },
        new()
        {
            Code = "D",
            Description = "Diverse"
        }
    };

    public Task<List<Employee>> GetDataAsync()
        => Task.FromResult( employees.Value
            .Select( x => new Employee( x ) ) //new() is used so we make sure that we are not returning the same item references avoiding an application wide "data corruption".
            .ToList() );

    private static List<Employee> LoadData()
    {
        Assembly assembly = typeof( EmployeeData ).Assembly;
        using var stream = assembly.GetManifestResourceStream( "Blazorise.Shared.Resources.EmployeeData.json" );
        return JsonSerializer.Deserialize<List<Employee>>( new StreamReader( stream ).ReadToEnd() );
    }
}