using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Blazorise.Utilities;
using Xunit;

namespace Blazorise.Tests.Utils;

public class ConvertersTests
{
    [Fact]
    public void ToDictionary_With_Null_Returns_Null()
    {
        // Act
        var result = Converters.ToDictionary( null );

        // Assert
        Assert.Null( result );
    }

    [Fact]
    public void ToDictionary_With_NullableProperties_Creates_Correct_Result()
    {
        // Arrange
        var test = new
        {
            integer = 42,
            integerWithDefault = default( int ),
            nullableInteger = (int?)43,
            nullableIntegerWithNull = (int?)null,
            stringValue = "test"
        };

        // Act
        var result = Converters.ToDictionary( test );

        // Assert
        Assert.NotNull( result );
        Assert.Equal( ["integer", "integerWithDefault", "nullableInteger", "stringValue"], result.Keys.OrderBy( x => x ) );
        Assert.Equal( 42, result["integer"] );
        Assert.Equal( 0, result["integerWithDefault"] );
        Assert.Equal( 43, result["nullableInteger"] );
        Assert.Equal( "test", result["stringValue"] );
    }

    [Fact]
    public void ToDictionary_With_DefaultValueProperties_Uses_EmitDefaultValue_To_Create_Correct_Result()
    {
        // Arrange
        var test = new Test
        {
            Integer = 42,
            IntegerWithDefault = default,
            IntegerWithDefaultOk = default,
            NullableInteger = 43,
            NullableIntegerWithNull = null
        };

        // Act
        var result = Converters.ToDictionary( test );

        // Assert
        Assert.NotNull( result );
        Assert.Equal( ["integer", "integerWithDefaultOk", "nullableInteger"], result.Keys.OrderBy( x => x ) );
        Assert.Equal( 42, result["integer"] );
        Assert.Equal( 0, result["integerWithDefaultOk"] );
        Assert.Equal( 43, result["nullableInteger"] );
    }

    [Fact]
    public void ToDictionary_With_NestedObject_Should_Create_Correct_Result()
    {
        // Arrange
        var test = new Test
        {
            Integer = 42,
            NestedTest = new()
            {
                Boolean = true,
                StringValue = "test"
            }
        };

        // Act
        var result = Converters.ToDictionary( test );

        // Assert
        Assert.NotNull( result );
        Assert.Equal( ["integer", "integerWithDefaultOk", "nestedTest"], result.Keys.OrderBy( x => x ) );
        Assert.Equal( 42, result["integer"] );
        Assert.Equal( 0, result["integerWithDefaultOk"] );

        var nested = result["nestedTest"] as Dictionary<string, object>;
        Assert.NotNull( nested );
        Assert.Equal( true, nested["boolean"] );
    }

    [Fact]
    public void ToDictionary_With_SimpleArray_Should_Create_Correct_Result()
    {
        // Arrange
        var test = new Test
        {
            Integer = 42,
            SimpleArray = [1, 2, 3]
        };

        // Act
        var result = Converters.ToDictionary( test );

        // Assert
        Assert.NotNull( result );
        Assert.Equal( ["integer", "integerWithDefaultOk", "simpleArray"], result.Keys.OrderBy( x => x ) );
        Assert.Equal( 42, result["integer"] );
        Assert.Equal( 0, result["integerWithDefaultOk"] );

        var simpleArray = result["simpleArray"] as object[];
        Assert.NotNull( simpleArray );
        Assert.Equal( [1, 2, 3], simpleArray );
    }

    [Fact]
    public void ToDictionary_With_SimpleList_Should_Create_Correct_Result()
    {
        // Arrange
        var test = new Test
        {
            Integer = 42,
            SimpleList = [1, 2, 3]
        };

        // Act
        var result = Converters.ToDictionary( test );

        // Assert
        Assert.NotNull( result );
        Assert.Equal( ["integer", "integerWithDefaultOk", "simpleList"], result.Keys.OrderBy( x => x ) );
        Assert.Equal( 42, result["integer"] );
        Assert.Equal( 0, result["integerWithDefaultOk"] );

        var simpleList = result["simpleList"] as List<object>;
        Assert.NotNull( simpleList );
        Assert.Equal( [1, 2, 3], simpleList.Cast<int>().ToList() );
    }

    [Fact]
    public void ToDictionary_With_ComplexArray_Should_Create_Correct_Result()
    {
        // Arrange
        var test = new Test
        {
            Integer = 42,
            ComplexArray =
            [
                new NestedTest { Boolean = true, StringValue = "test1" },
                new NestedTest { Boolean = true, StringValue = null },
                new NestedTest { Boolean = false, StringValue = "test3" }
            ]
        };

        // Act
        var result = Converters.ToDictionary( test );

        // Assert
        Assert.NotNull( result );
        Assert.Equal( ["complexArray", "integer", "integerWithDefaultOk"], result.Keys.OrderBy( x => x ) );
        Assert.Equal( 42, result["integer"] );
        Assert.Equal( 0, result["integerWithDefaultOk"] );

        var nestedArray = result["complexArray"] as object[];
        Assert.NotNull( nestedArray );
        Assert.Collection( nestedArray, _ => { }, _ => { }, _ => { } );

        var arrayItem1 = (Dictionary<string, object>)nestedArray[0];
        Assert.Equal( ["boolean", "stringValue"], arrayItem1.Keys.OrderBy( x => x ) );
        Assert.Equal( true, arrayItem1["boolean"] );
        Assert.Equal( "test1", arrayItem1["stringValue"] );

        var arrayItem2 = (Dictionary<string, object>)nestedArray[1];
        Assert.Single( arrayItem2 );
        Assert.Equal( true, arrayItem2["boolean"] );

        var arrayItem3 = (Dictionary<string, object>)nestedArray[2];
        Assert.Single( arrayItem3 );
        Assert.Equal( "test3", arrayItem3["stringValue"] );
    }

    [Fact]
    public void ToDictionary_With_ComplexList_Should_Create_Correct_Result()
    {
        // Arrange
        var test = new Test
        {
            Integer = 42,
            ComplexList =
            [
                new() { Boolean = true, StringValue = "test1" },
                new() { Boolean = true, StringValue = null },
                new() { Boolean = false, StringValue = "test3" }
            ]
        };

        // Act
        var result = Converters.ToDictionary( test );

        // Assert
        Assert.NotNull( result );
        Assert.Equal( ["complexList", "integer", "integerWithDefaultOk"], result.Keys.OrderBy( x => x ) );
        Assert.Equal( 42, result["integer"] );
        Assert.Equal( 0, result["integerWithDefaultOk"] );

        var complexList = result["complexList"] as List<object>;
        Assert.NotNull( complexList );
        Assert.Collection( complexList, _ => { }, _ => { }, _ => { } );

        var item1 = (Dictionary<string, object>)complexList[0];
        Assert.Equal( ["boolean", "stringValue"], item1.Keys.OrderBy( x => x ) );
        Assert.Equal( true, item1["boolean"] );
        Assert.Equal( "test1", item1["stringValue"] );

        var item2 = (Dictionary<string, object>)complexList[1];
        Assert.Single( item2 );
        Assert.Equal( true, item2["boolean"] );

        var item3 = (Dictionary<string, object>)complexList[2];
        Assert.Single( item3 );
        Assert.Equal( "test3", item3["stringValue"] );
    }

    [Fact]
    public void ToDictionary_With_DataMemberName_Should_Use_That_Name()
    {
        // Arrange
        var test = new TestWithDataMemberName
        {
            Foo = "abc"
        };

        // Act
        var result = Converters.ToDictionary( test );

        // Assert
        Assert.NotNull( result );
        var entry = Assert.Single( result );
        Assert.Equal( "foobar", entry.Key );
        Assert.Equal( "abc", entry.Value );
    }

    [Fact]
    public void ToDictionary_With_ForceCamelCase_Is_False_Should_Use_Original_DataMemberName()
    {
        // Arrange
        var test = new TestWithDataMemberName
        {
            Foo = "abc"
        };

        // Act
        var result = Converters.ToDictionary( test, true, false );

        // Assert
        Assert.NotNull( result );
        var entry = Assert.Single( result );
        Assert.Equal( "Foobar", entry.Key );
        Assert.Equal( "abc", entry.Value );
    }

    [Fact]
    public void ToDictionary_With_ForceCamelCase_Is_False_Should_Use_Original_PropertyName()
    {
        // Arrange
        var test = new
        {
            Foo = "abc"
        };

        // Act
        var result = Converters.ToDictionary( test, true, false );

        // Assert
        Assert.NotNull( result );
        var entry = Assert.Single( result );
        Assert.Equal( "Foo", entry.Key );
        Assert.Equal( "abc", entry.Value );
    }

    [Fact]
    public void ToDictionary_With_CallbackExpression_ConvertsExpression_To_TemplatedStringLiteral()
    {
        // Arrange
        var test = new Test
        {
            Callback = ( value, index, values ) => $"{value / 1000} K"
        };

        // Act
        var result = Converters.ToDictionary( test, false );

        // Assert
        Assert.NotNull( result );
        Assert.Equal( "`${(value / 1000)} K`", result["callback"] );
    }

    [Theory]
    [InlineData( "2020-08-24", true )]
    [InlineData( "not a date", false )]
    public void TryChangeType_With_DateOnly_String_As_Value_Should_BeExpected( string value, bool expected )
    {
        // Arrange

        // Act
        var result = Converters.TryChangeType<DateOnly>( value, out var _ );

        // Assert
        Assert.Equal( expected, result );
    }

    [Fact]
    public void TryChangeType_With_DateOnly_Current_Culture()
    {
        // Arrange
        var test = DateOnly.FromDateTime( DateTime.Now ).ToString();

        // Act
        Converters.TryChangeType<DateOnly>( test, out var result );

        // Assert
        Assert.Equal( test, result.ToString() );
    }

    [Theory]
    [InlineData( "2020-08-24T17:48:00-04:00", true )]
    [InlineData( "not a date", false )]
    public void TryChangeType_With_DateTimeOffset_String_As_Value_Should_BeExpected( string value, bool expected )
    {
        // Arrange

        // Act
        var result = Converters.TryChangeType<DateTimeOffset>( value, out var _ );

        // Assert
        Assert.Equal( expected, result );
    }

    [Fact]
    public void TryChangeType_With_DateTimeOffset_Current_Culture()
    {
        // Arrange
        var test = DateTimeOffset.Now.ToString();

        // Act
        Converters.TryChangeType<DateTimeOffset>( test, out var result );

        // Assert
        Assert.Equal( test, result.ToString() );
    }
}

public class Test
{
    [DataMember( EmitDefaultValue = false )]
    public int Integer { get; set; }

    [DataMember( EmitDefaultValue = false )]
    public int IntegerWithDefault { get; set; }

    public int IntegerWithDefaultOk { get; set; }

    [DataMember( EmitDefaultValue = false )]
    public int? NullableInteger { get; set; }

    [DataMember( EmitDefaultValue = false )]
    public int? NullableIntegerWithNull { get; set; }

    [DataMember( EmitDefaultValue = false )]
    public NestedTest NestedTest { get; set; }

    [DataMember( EmitDefaultValue = false )]
    public int[] SimpleArray { get; set; }

    [DataMember( EmitDefaultValue = false )]
    public List<int> SimpleList { get; set; }

    [DataMember( EmitDefaultValue = false )]
    public NestedTest[] ComplexArray { get; set; }

    [DataMember( EmitDefaultValue = false )]
    public List<NestedTest> ComplexList { get; set; }

    [DataMember( EmitDefaultValue = false )]
    public Expression<Func<double, int, double[], FormattableString>> Callback { get; set; }
}

public class NestedTest
{
    [DataMember( EmitDefaultValue = false )]
    public bool Boolean { get; set; }

    [DataMember( EmitDefaultValue = false )]
    public string StringValue { get; set; }
}

public class TestWithDataMemberName
{
    [DataMember( Name = "Foobar" )]
    public string Foo { get; set; }
}