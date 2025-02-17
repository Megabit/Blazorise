using System;
using System.Globalization;
using Blazorise.DataGrid.Utils;
using Xunit;

namespace Blazorise.Tests.DataGrid.Utils;

public class FunctionCompilerTests
{
    [Theory]
    [InlineData( "Id", "0" )]
    [InlineData( "Name", null )]
    [InlineData( "Value", null )]
    [InlineData( "Boolean", "False" )]
    [InlineData( "DateTime", "1/1/0001 12:00:00 AM" )]
    [InlineData( "DateTime.TimeOfDay", "00:00:00" )]
    [InlineData( "DateTimeNull", null )]
    [InlineData( "TimeSpan", "00:00:00" )]
    [InlineData( "TimeSpanNull", null )]
    [InlineData( "Information.Id", "0" )]
    [InlineData( "Information.Message", null )]
    [InlineData( "Information.Detail.Id", "0" )]
    [InlineData( "Information.Detail.Description", null )]
    public void ValueGetter_ReturnsCorrectPropertyOrField_DefaultValue( string field, string expected )
    {
        var test = new Test();
        var valueGetter = FunctionCompiler.CreateValueGetter<Test>( field );
        var value = valueGetter( test );

        Assert.Equal( expected, value switch
        {
            DateTime dt => dt.ToString( "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture ),
            _ => value?.ToString()
        } );
    }

    [Theory]
    [InlineData( "Id", "100" )]
    [InlineData( "Name", "John" )]
    [InlineData( "Value", "200" )]
    [InlineData( "Boolean", "True" )]
    [InlineData( "DateTime", "12/31/9999 11:59:59 PM" )]
    [InlineData( "DateTime.TimeOfDay", "23:59:59.9999999" )]
    [InlineData( "DateTimeNull", "8/18/2018 12:00:00 AM" )]
    [InlineData( "TimeSpan", "10675199.02:48:05.4775807" )]
    [InlineData( "TimeSpanNull", "20:00:00" )]
    [InlineData( "Information.Id", "1000" )]
    [InlineData( "Information.Message", "This is a message!" )]
    [InlineData( "Information.Detail.Id", "2000" )]
    [InlineData( "Information.Detail.Description", "This is a description!" )]
    public void ValueGetter_ReturnsCorrectPropertyOrField_ActualValue( string field, string expected )
    {
        var test = GetTest();
        var valueGetter = FunctionCompiler.CreateValueGetter<Test>( field );
        var value = valueGetter( test );

        Assert.Equal( expected, value switch
        {
            DateTime dt => dt.ToString( "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture ),
            _ => value?.ToString()
        } );
    }

    public FunctionCompilerTests()
    {
        // force to use us culture info
        CultureInfo.CurrentCulture = new CultureInfo( "en-US" );
    }

    private Test GetTest()
    {
        return new()
        {
            Id = 100,
            Name = "John",
            Value = 200,
            Boolean = true,
            DateTime = DateTime.MaxValue,
            DateTimeNull = DateTime.Parse( "08/18/2018" ),
            TimeSpan = TimeSpan.MaxValue,
            TimeSpanNull = TimeSpan.FromHours( 20.0 ),
            Information = new()
            {
                Id = 1000,
                Message = "This is a message!",
                Detail = new()
                {
                    Id = 2000,
                    Description = "This is a description!"
                }
            }

        };
    }

    private class Test
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public int? Value { get; set; }
        public bool Boolean { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime? DateTimeNull { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public TimeSpan? TimeSpanNull { get; set; }
        public Information Information { get; set; }
    }

    private class Information
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public Detail Detail { get; set; }

    }

    private class Detail
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
}