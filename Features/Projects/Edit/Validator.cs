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
            RuleFor(p => p.ProductionCompany).MaximumLength(100);
            RuleFor(p => p.ReferenceCode).MaximumLength(50);

            RuleFor(p => p.StartDate).NotEmpty();
            RuleFor(p => p.EndDate)
                .GreaterThan(x => x.StartDate)
                .When(x => x.EndDate.HasValue);

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

            Func<Command, bool> isInvoiceAddressPartiallyFilled = cmd =>
                !string.IsNullOrWhiteSpace(cmd.InvoiceStreet) ||
                !string.IsNullOrWhiteSpace(cmd.InvoiceCity) ||
                !string.IsNullOrWhiteSpace(cmd.InvoiceZipCode) ||
                !string.IsNullOrWhiteSpace(cmd.InvoiceCountry) ||
                !string.IsNullOrWhiteSpace(cmd.InvoiceProvince);

            When(isInvoiceAddressPartiallyFilled, () =>
            {
                RuleFor(x => x.InvoiceStreet).NotEmpty().WithMessage("Invoice street is required for a complete invoice address.");
                RuleFor(x => x.InvoiceCity).NotEmpty().WithMessage("Invoice city is required for a complete invoice address.");
                RuleFor(x => x.InvoiceZipCode).NotEmpty().WithMessage("Invoice Zip Code is required for a complete invoice address.");
                RuleFor(x => x.InvoiceCountry).NotEmpty().WithMessage("Invoice country is required for a complete invoice address.");
            });
        }
    }
}
