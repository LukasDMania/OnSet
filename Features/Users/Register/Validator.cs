using FluentValidation;

namespace OnSet.Features.Users.Register
{
    /// <summary>FluentValidation rules for this feature slice.</summary>
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            // --- 1. Required Identity and Domain Fields ---

            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .Equal(x => x.Password).WithMessage("Passwords do not match.");

            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);

            // --- 2. Conditional Address VO Validation ---


            Func<Command, bool> isAddressPartiallyFilled = cmd =>
                !string.IsNullOrWhiteSpace(cmd.Street) ||
                !string.IsNullOrWhiteSpace(cmd.City) ||
                !string.IsNullOrWhiteSpace(cmd.ZipCode) ||
                !string.IsNullOrWhiteSpace(cmd.Country);

            When(isAddressPartiallyFilled, () =>
            {
                RuleFor(x => x.Street).NotEmpty().WithMessage("Street is required for a complete address.");
                RuleFor(x => x.City).NotEmpty().WithMessage("City is required for a complete address.");
                RuleFor(x => x.ZipCode).NotEmpty().WithMessage("Zip Code is required for a complete address.");
                RuleFor(x => x.Country).NotEmpty().WithMessage("Country is required for a complete address.");
            });

            // --- 3. Conditional EmergencyContact VO Validation ---

            Func<Command, bool> isEmergencyContactPartiallyFilled = cmd =>
                !string.IsNullOrWhiteSpace(cmd.EmergencyContactName) ||
                !string.IsNullOrWhiteSpace(cmd.EmergencyContactPhone);

            When(isEmergencyContactPartiallyFilled, () =>
            {
                RuleFor(x => x.EmergencyContactName).NotEmpty().WithMessage("Emergency contact name is required.");
                RuleFor(x => x.EmergencyContactPhone).NotEmpty().WithMessage("Emergency contact phone is required.");
            });

            RuleFor(x => x.Bio).MaximumLength(500);
        }
    }
}
