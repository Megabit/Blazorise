using Xunit;

namespace Blazorise.Tests.Utils;

public class DeepClonerTests
{
    [Fact]
    public void DeepCloner_Should_Return_NewClonedReference()
    {
        // Arrange
        var obj = new Test()
        {
            Integer = 1,
            String = "abc",
            TestCircular = new Test()
            {

            },
            TestNested = new TestNested()
            {
                Integer = 2,
                String = "def",
            }
        };

        // Act
        var clonedObj = obj.DeepClone();

        // Assert
        Assert.False( object.ReferenceEquals( obj, clonedObj ) );
        Assert.Equal( obj.Integer, clonedObj.Integer );
        Assert.Equal( obj.String, clonedObj.String );

        Assert.False( object.ReferenceEquals( obj.TestNested, clonedObj.TestNested ) );
        Assert.Equal( obj.TestNested.Integer, clonedObj.TestNested.Integer );
        Assert.Equal( obj.TestNested.String, clonedObj.TestNested.String );
    }

    [Fact]
    public void DeepCloner_Should_NotThrowOrHang_When_CircularReference()
    {
        // Arrange
        var obj = new Test()
        {
            Integer = 1,
            String = "abc",
            TestCircular = new Test()
            {

            },
            TestNested = new TestNested()
            {
                Integer = 2,
                String = "def",
            }
        };
        obj.TestCircular = obj;

        // Act
        var clonedObj = obj.DeepClone();

        // Assert
        Assert.False( object.ReferenceEquals( obj, clonedObj ) );
    }

    private class Test
    {
        public string String { get; set; }
        public int Integer { get; set; }

        public Test TestCircular { get; set; }
        public TestNested TestNested { get; set; }
    }

    private class TestNested
    {
        public string String { get; set; }
        public int Integer { get; set; }
    }

}

