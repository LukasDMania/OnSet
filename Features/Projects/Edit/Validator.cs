using FluentValidation;

namespace OnSet.Features.Projects.Edit
{
    /// <summary>FluentValidation rules for this feature slice.</summary>
    public class Validator : AbstractValidator<Command>
    {
        public Validator() 
        {
            RuleFor(p => p.Name).NotEmpty().MaximumLength(100);
            RuleFor(p => p.Description).MaximumLength(500);
            RuleFor(p => p.ClientName).MaximumLength(100);
            RuleFor(p => p.ReferenceCode).MaximumLength(50);

            RuleFor(p => p.StartDate).NotEmpty();
            RuleFor(p => p.EndDate)
                .GreaterThan(x => x.StartDate)
                .When(x => x.EndDate.HasValue);

            RuleFor(p => p.Budget)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Budget.HasValue);

            Func<Command, bool> isAddressPartiallyFilled = cmd =>
                !string.IsNullOrWhiteSpace(cmd.Street) ||
                !string.IsNullOrWhiteSpace(cmd.City) ||
                !string.IsNullOrWhiteSpace(cmd.ZipCode) ||
                !string.IsNullOrWhiteSpace(cmd.Country) ||
                !string.IsNullOrWhiteSpace(cmd.Province);

            When(isAddressPartiallyFilled, () =>
            {
                RuleFor(x => x.Street).NotEmpty().WithMessage("Street is required for a complete address.");
                RuleFor(x => x.City).NotEmpty().WithMessage("City is required for a complete address.");
                RuleFor(x => x.ZipCode).NotEmpty().WithMessage("Zip Code is required for a complete address.");
                RuleFor(x => x.Country).NotEmpty().WithMessage("Country is required for a complete address.");
            });
        }
    }
}
