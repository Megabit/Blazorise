using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blazorise.Tests.Utils;

public class ValidationMessageLocalizerAttributeFinderTests
{
    [Fact]
    public void LocalizedMessage_UsesExpectedArgumentOrder_ForMinLength()
    {
        var model = new TestModel();
        var editContext = new EditContext( model );
        var messages = new ValidationMessageStore( editContext );
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var validator = new EditContextValidator( new ValidationMessageLocalizerAttributeFinder(), serviceProvider );

        string Localizer( string message, IEnumerable<string> args )
        {
            var argumentList = args?.ToArray() ?? Array.Empty<string>();
            return string.Format( message, argumentList );
        }

        var fieldIdentifier = new FieldIdentifier( model, nameof( TestModel.Name ) );

        validator.ValidateField( editContext, messages, fieldIdentifier, Localizer );

        var localizedMessage = editContext.GetValidationMessages( fieldIdentifier ).Single();
        var expectedMessage = GetExpectedMinLengthMessage( model );

        Assert.Equal( expectedMessage, localizedMessage );
    }

    private static string GetExpectedMinLengthMessage( TestModel model )
    {
        var validationContext = new ValidationContext( model )
        {
            MemberName = nameof( TestModel.Name ),
        };
        var results = new List<ValidationResult>();

        Validator.TryValidateValue( model.Name, validationContext, results, new[] { new MinLengthAttribute( 2 ) } );

        return results.Single().ErrorMessage;
    }

    private sealed class TestModel
    {
        [MinLength( 2 )]
        public string Name { get; set; } = "a";
    }
}