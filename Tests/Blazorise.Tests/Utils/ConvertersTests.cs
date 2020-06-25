using System.Runtime.Serialization;
using Blazorise.Utils;
using Xunit;

namespace Blazorise.Tests.Utils
{
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
                nullableIntegerWithNull = (int?)null
            };

            // Act
            var result = Converters.ToDictionary( test );

            // Assert
            Assert.NotNull( result );
            Assert.Equal( 3, result.Count );
            Assert.Equal( 42, result["integer"] );
            Assert.Equal( 0, result["integerWithDefault"] );
            Assert.Equal( 43, result["nullableInteger"] );
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
            Assert.Equal( 3, result.Count );
            Assert.Equal( 42, result["integer"] );
            Assert.Equal( 0, result["integerWithDefaultOk"] );
            Assert.Equal( 43, result["nullableInteger"] );
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
            Assert.Equal( 1, result.Count );
            Assert.Equal( "abc", result["foobar"] );
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
            Assert.Equal( 1, result.Count );
            Assert.Equal( "abc", result["Foobar"] );
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
            Assert.Equal( 1, result.Count );
            Assert.Equal( "abc", result["Foo"] );
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
    }

    public class TestWithDataMemberName
    {
        [DataMember( Name = "Foobar" )]
        public string Foo { get; set; }
    }
}
