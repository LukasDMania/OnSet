using FluentValidation;

namespace OnSet.Features.Projects.Create
{
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ProjectName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleFor(x => x.ClientName)
                .MaximumLength(100);

            RuleFor(x => x.ReferenceCode)
                .MaximumLength(50);

            RuleFor(x => x.StartDate)
                .NotEmpty();

            RuleFor(x => x.Budget)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Budget.HasValue);

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .When(x => x.EndDate.HasValue);



            Func<Command, bool> isAddressPartiallyFilled = cmd =>
                !string.IsNullOrWhiteSpace(cmd.Street) ||
                !string.IsNullOrWhiteSpace(cmd.City) ||
                !string.IsNullOrWhiteSpace(cmd.ZipCode) ||
                !string.IsNullOrWhiteSpace(cmd.Country);

            When(isAddressPartiallyFilled, () =>
            {
                RuleFor(x => x.Street)
                    .NotEmpty().WithMessage("Street is required for a complete address.");

                RuleFor(x => x.City)
                    .NotEmpty().WithMessage("City is required for a complete address.");

                RuleFor(x => x.ZipCode)
                    .NotEmpty().WithMessage("Zip Code is required for a complete address.");

                RuleFor(x => x.Country)
                    .NotEmpty().WithMessage("Country is required for a complete address.");
            });
        }
    }
}
