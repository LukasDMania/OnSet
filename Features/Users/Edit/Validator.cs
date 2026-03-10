using FluentValidation;

namespace OnSet.Features.Users.Edit
{
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);

            RuleFor(x => x.YearsExperience)
                .InclusiveBetween(0, 80)
                .When(x => x.YearsExperience.HasValue);

            RuleFor(x => x.Bio)
                .MaximumLength(500);

            RuleFor(x => x.AvatarUrl)
                .Must(uri => string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("Invalid URL format for Avatar.");

            RuleFor(x => x.NextAvailableDate)
                .GreaterThan(DateTime.Today)
                .When(x => x.IsAvailableForBooking && x.NextAvailableDate.HasValue)
                .WithMessage("Next available date must be in the future.");

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

            Func<Command, bool> isEmergencyContactPartiallyFilled = cmd =>
                !string.IsNullOrWhiteSpace(cmd.EmergencyContactName) ||
                !string.IsNullOrWhiteSpace(cmd.EmergencyContactPhone);

            When(isEmergencyContactPartiallyFilled, () =>
            {
                RuleFor(x => x.EmergencyContactName).NotEmpty().WithMessage("Emergency contact name is required.");
                RuleFor(x => x.EmergencyContactPhone).NotEmpty().WithMessage("Emergency contact phone is required.");
            });
        }
    }
}
