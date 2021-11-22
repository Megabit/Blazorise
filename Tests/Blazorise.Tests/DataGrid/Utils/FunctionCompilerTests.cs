using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Blazorise.DataGrid;
using Blazorise.DataGrid.Utils;
using Xunit;

namespace Blazorise.Tests.DataGrid.Utils
{
    public class FunctionCompilerTests
    {
        [Theory]
        [InlineData("Id", "0")]
        [InlineData( "Name", null )]
        [InlineData( "Value", null )]
        [InlineData( "Boolean", "False" )]
        [InlineData( "DateTime", "1/1/0001 12:00:00 AM" )]
        [InlineData( "DateTime.TimeOfDay", "00:00:00" )]
        [InlineData( "Information.Id", "0" )]
        [InlineData( "Information.Message", null )]
        [InlineData( "Information.Detail.Id", "0" )]
        [InlineData( "Information.Detail.Description", null )]
        public void ValueGetter_ReturnsCorrectPropertyOrField_DefaultValue(string field, string expected)
        {
            var test = new Test( );
            var valueGetter = FunctionCompiler.CreateValueGetter<Test>( field );

            Assert.Equal(expected, valueGetter( test )?.ToString());
        }

        [Theory]
        [InlineData( "Id", "100" )]
        [InlineData( "Name", "John" )]
        [InlineData( "Value", "200" )]
        [InlineData( "Boolean", "True" )]
        [InlineData( "DateTime", "12/31/9999 11:59:59 PM" )]
        [InlineData( "DateTime.TimeOfDay", "23:59:59.9999999" )]
        [InlineData( "Information.Id", "1000" )]
        [InlineData( "Information.Message", "This is a message!" )]
        [InlineData( "Information.Detail.Id", "2000" )]
        [InlineData( "Information.Detail.Description", "This is a description!" )]
        public void ValueGetter_ReturnsCorrectPropertyOrField_ActualValue( string field, string expected )
        {
            var test = GetTest();
            var valueGetter = FunctionCompiler.CreateValueGetter<Test>( field );

            Assert.Equal( expected, valueGetter( test )?.ToString() );
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
}