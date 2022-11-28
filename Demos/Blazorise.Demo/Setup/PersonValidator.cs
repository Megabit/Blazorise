using Blazorise.Shared.Models;
using FluentValidation;

namespace Blazorise.Demo.Setup;

public class PersonValidator : AbstractValidator<Person>
{
    public PersonValidator()
    {
        RuleFor( vm => vm.FirstName )
            .NotEmpty()
            .MaximumLength( 30 );

        RuleFor( vm => vm.LastName )
            .NotEmpty()
            .MaximumLength( 30 );

        RuleFor( vm => vm.Age )
            .GreaterThanOrEqualTo( 18 );
    }
}