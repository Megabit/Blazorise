using Blazorise.Shared.Models;
using FluentValidation;

namespace Blazorise.Docs.Pages.Docs.Extensions.FluentValidation.Validators
{
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
}
