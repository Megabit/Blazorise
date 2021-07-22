using System;
using System.Linq.Expressions;
using Blazorise.Utilities;
using Xunit;

namespace Blazorise.Tests.Utils
{
    public class ExpressionConverterTests
    {
        [Fact]
        public void ToTemplatedStringLiteral_With_ParameterAsIntegerValue()
        {
            // Arrange
            Expression<Func<int, FormattableString>> expression = ( value ) => $"{value}";

            // Act
            var result = ExpressionConverter.ToTemplatedStringLiteral( expression );

            // Assert
            Assert.Equal( "`${(value)}`", result );
        }

        [Fact]
        public void ToTemplatedStringLiteral_With_ParameterAsStringValue()
        {
            // Arrange
            Expression<Func<string, FormattableString>> expression = ( value ) => $"{value}";

            // Act
            var result = ExpressionConverter.ToTemplatedStringLiteral( expression );

            // Assert
            Assert.Equal( "`${(value)}`", result );
        }

        [Fact]
        public void ToTemplatedStringLiteral_With_ConditionalExpression()
        {
            // Arrange
            Expression<Func<int, FormattableString>> expression = ( value ) => $"{( value < 1000 ? value : ( value / 1000.0 ) + " K" )}";

            // Act
            var result = ExpressionConverter.ToTemplatedStringLiteral( expression );

            // Assert
            Assert.Equal( "`${((value < 1000) ? (value) : (value / 1000 + ' K'))}`", result );
        }

        [Fact]
        public void ToTemplatedStringLiteral_With_ConstantValue()
        {
            // Arrange
            Expression<Func<int, FormattableString>> expression = ( value ) => $"abc";

            // Act
            var result = ExpressionConverter.ToTemplatedStringLiteral( expression );

            // Assert
            Assert.Equal( "`abc`", result );
        }

        [Fact]
        public void ToTemplatedStringLiteral_With_OperatorAddNumbers()
        {
            // Arrange
            Expression<Func<int, double, FormattableString>> expression = ( value1, value2 ) => $"{value1 + value2}";

            // Act
            var result = ExpressionConverter.ToTemplatedStringLiteral( expression );

            // Assert
            Assert.Equal( "`${(value1 + value2)}`", result );
        }

        [Fact]
        public void ToTemplatedStringLiteral_With_OperatorAddStrings()
        {
            // Arrange
            Expression<Func<string, string, FormattableString>> expression = ( string1, string2 ) => $"{string1 + string2}";

            // Act
            var result = ExpressionConverter.ToTemplatedStringLiteral( expression );

            // Assert
            Assert.Equal( "`${(string1 + string2)}`", result );
        }

        [Fact]
        public void ToTemplatedStringLiteral_With_OperatorAddNumberAndStrings()
        {
            // Arrange
            Expression<Func<int, string, FormattableString>> expression = ( number, str ) => $"{number + str + "X"}";

            // Act
            var result = ExpressionConverter.ToTemplatedStringLiteral( expression );

            // Assert
            Assert.Equal( "`${(number + str + 'X')}`", result );
        }

        [Fact]
        public void ToTemplatedStringLiteral_With_OperatorArray()
        {
            // Arrange
            Expression<Func<int[], FormattableString>> expression = ( values ) => $"{values[0]}";

            // Act
            var result = ExpressionConverter.ToTemplatedStringLiteral( expression );

            // Assert
            Assert.Equal( "`${(values[0])}`", result );
        }
    }
}
